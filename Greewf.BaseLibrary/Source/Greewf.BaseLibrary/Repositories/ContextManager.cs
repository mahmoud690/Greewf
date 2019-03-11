﻿using effts;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Threading.Tasks;
using System.Transactions;

namespace Greewf.BaseLibrary.Repositories
{
    public abstract class ContextManagerBase
    {
        public DbContext ContextBase { get; protected set; } //we need this in ChangesTracker module.     
                                                             // public event Action OnChangesCommitted;

        //protected internal void InvokeOnChangesCommittedEvent()
        //{
        //    OnChangesCommitted?.Invoke();
        //}
    }

    public abstract class ContextManager<T> : ContextManagerBase
        where T : DbContext, ITransactionScopeAwareness, new()
    {
        private static bool _isFullTextSearchInitiated = false;

        public IValidationDictionary ValidationDictionary { get; private set; }

        public T Context { get; private set; }
        public ContextManager(IValidationDictionary validationDictionary, Func<T> contextInstantiator = null)
        {
            Context = contextInstantiator == null ? new T() : contextInstantiator();
            ContextBase = Context;
            ValidationDictionary = validationDictionary ?? new DefaultValidationDictionary();
            if (!_isFullTextSearchInitiated)
            {
                _isFullTextSearchInitiated = true;
                DbInterception.Add(new FtsInterceptor());
            }
        }

        public virtual bool SaveChanges()
        {
            return SaveChangesBase(() => Context.SaveChanges());
        }

        public virtual Task<bool> SaveChangesAsync()
        {
            return Task.FromResult(SaveChangesBase(() => Context.SaveChangesAsync()));
        }

        private bool SaveChangesBase(Action contextSaveChangesFunction)
        {
            if (ValidationDictionary != null && !ValidationDictionary.IsValid)
                return false;

            bool result = true;

            //to avoid creating new scope (it comes when savechanges causes to call savechanges again). 
            //شرط دوم برای این است که ممکن است کانتکست از ابتدا با یک کانکشنی ایجاد شده باشد که خود در درون ترنزکشن است
            //در این حالت ایجاد ترنزکشن اسکوپ باعث می شود خطای روبرو را بگیریم : "Cannot enlist in the transaction because a local transaction is in progress on the connection"
            if (Context.IsInActiveTransactionScope || Context.Database.CurrentTransaction != null)
            {
                contextSaveChangesFunction();
                if (ValidationDictionary?.IsValid == false)
                    result = false;

                return result;
            }

            try
            {
                Context.OnBeforeTransactionScopeStart();

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled/*due to https://stackoverflow.com/q/13543254/790811 to be able to support async cases*/ ))
                {
                    Context.IsInActiveTransactionScope = true;
                    contextSaveChangesFunction();//my be some calls on onChangesSaved event that needs to be in transaction

                    if (ValidationDictionary?.IsValid == false)
                        result = false;

                    if (result)
                        scope.Complete();
                    else
                        scope.Dispose();
                }
            }
            finally
            {
                Context.IsInActiveTransactionScope = false;
            }



            if (result)
                Context.OnTransactionScopeCommitted();
            else
                Context.OnTransactionScopeRollbacked();

            return result;
        }

    }
}
