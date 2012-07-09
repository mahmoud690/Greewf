﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greewf.BaseLibrary.MVC.Security;
using System.Web.Routing;
using System.Text;
using System.Data.Entity;
using Greewf.BaseLibrary.Repositories;
using Greewf.BaseLibrary.MVC.Logging;

namespace Greewf.BaseLibrary.MVC
{
    public abstract class CustomizedControllerBase : Controller
    {
        private const string SavedSuccessfullyUrlFormat = "~/home/SavedSuccessfully?url={0}";
        private const string SavedSuccessfullyControllerName = "home";
        internal const string SavedSuccessfullyActionName = "SavedSuccessfully";
        internal const string SavedSuccessfullyFramgment = "successfullysaved";

        private new RedirectResult Redirect(string url)
        {
            return Redirect(url, false, null);
        }

        protected RedirectResult Redirect(string url, object model)
        {
            return Redirect(url, false, model);
        }

        protected RedirectResult Redirect(string url, bool setSaveSuccesfullyFlag, object model)
        {
            ViewData.Model = model;
            url = CheckRedirectAddress(url, setSaveSuccesfullyFlag);
            url = EnsureWindowFlag(url);
            url = EnsureSaveFlag(url, setSaveSuccesfullyFlag);

            return base.Redirect(url);

        }

        private string EnsureSaveFlag(string url, bool setSaveSuccesfullyFlag)
        {
            if (setSaveSuccesfullyFlag == false) return url;
            if (url.Contains("#"))
                return url + ";" + SavedSuccessfullyFramgment;//surely the hash segment is placed at the end of url , so we add our string to it simply
            else
                return url + "#" + SavedSuccessfullyFramgment;
        }

        private RedirectToRouteResult EnsureSaveFlag(RedirectToRouteResult result, bool setSaveSuccesfullyFlag)
        {
            if (setSaveSuccesfullyFlag == false) return result;
            return result.AddFragment(SavedSuccessfullyFramgment);
        }

        private string CheckRedirectAddress(string url, bool setSaveSuccesfullyFlag)
        {
            if (IsCurrentRequestRunInWindow && setSaveSuccesfullyFlag)//when the current request is in window and it is savedsucessfully, we redirect it to home/SavedSuccessfully page automatically
                return string.Format(SavedSuccessfullyUrlFormat, url);
            return url;
        }

        private RedirectToRouteResult CheckRedirectAddress(RedirectToRouteResult result, bool setSaveSuccesfullyFlag)
        {
            if (IsCurrentRequestRunInWindow && setSaveSuccesfullyFlag)//when the current request is in window and it is savedsucessfully, we redirect it to home/SavedSuccessfully page automatically
                return base.RedirectToAction(SavedSuccessfullyActionName, SavedSuccessfullyControllerName, new { url = result.ToString(this.ControllerContext) });
            return result;
        }


        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            return this.RedirectToAction(actionName, controllerName, routeValues, false);
        }

        protected RedirectToRouteResult RedirectToSuccessAction(object model)
        {
            ViewData.Model = model;
            var result = base.RedirectToAction(SavedSuccessfullyActionName, SavedSuccessfullyControllerName);
            return this.CorrectRedirectToRouteResult(result, true);
        }

        protected RedirectToRouteResult RedirectToSuccessAction(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues, object model)
        {
            ViewData.Model = model;
            var result = base.RedirectToAction(actionName, controllerName, routeValues);
            return this.CorrectRedirectToRouteResult(result, true);
        }

        protected RedirectToRouteResult RedirectToSuccessAction(string actionName, string controllerName, object routeValues, object model)
        {
            ViewData.Model = model;
            var result = base.RedirectToAction(actionName, controllerName, routeValues);
            return this.CorrectRedirectToRouteResult(result, true);
        }

        protected RedirectToRouteResult RedirectToSuccessAction(string actionName, object routeValues, object model)
        {
            ViewData.Model = model;
            var result = base.RedirectToAction(actionName, routeValues);
            return this.CorrectRedirectToRouteResult(result, true);
        }

        protected RedirectToRouteResult RedirectToAction(string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues, bool setSaveSuccesfullyFlag)
        {
            var result = base.RedirectToAction(actionName, controllerName, routeValues);
            return CorrectRedirectToRouteResult(result, setSaveSuccesfullyFlag);

        }

        private RedirectToRouteResult CorrectRedirectToRouteResult(RedirectToRouteResult result, bool setSaveSuccesfullyFlag)
        {

            result = CheckRedirectAddress(result, setSaveSuccesfullyFlag);
            EnsureWindowFlag(result.RouteValues);
            result = EnsureSaveFlag(result, setSaveSuccesfullyFlag);

            return result;
        }

        protected override RedirectToRouteResult RedirectToRoute(string routeName, System.Web.Routing.RouteValueDictionary routeValues)
        {
            EnsureWindowFlag(routeValues);
            return base.RedirectToRoute(routeName, routeValues);
        }

        private void EnsureWindowFlag(System.Web.Routing.RouteValueDictionary routeValues)
        {
            //this method should also ensures Puremode and Simplemode too
            EnsureFlag(routeValues, IsCurrentRequestRunInWindow, "iswindow");
            EnsureFlag(routeValues, Request.QueryString.AllKeys.Contains("puremode"), "puremode");
            EnsureFlag(routeValues, Request.QueryString.AllKeys.Contains("simplemode"), "simplemode");
            EnsureFlag(routeValues, Request.QueryString.AllKeys.Contains("includeUrlInContent"), "includeUrlInContent");

        }

        private string EnsureWindowFlag(string url)
        {
            //this method should also ensures Puremode and Simplemode too
            url = EnsureFlag(url, IsCurrentRequestRunInWindow, "iswindow");
            url = EnsureFlag(url, Request.QueryString.AllKeys.Contains("puremode"), "puremode");
            url = EnsureFlag(url, Request.QueryString.AllKeys.Contains("simplemode"), "simplemode");
            url = EnsureFlag(url, Request.QueryString.AllKeys.Contains("includeUrlInContent"), "includeUrlInContent");

            return url;
        }

        private string EnsureFlag(string url, bool applyFlag, string flag)
        {
            if (applyFlag && url.IndexOf(flag) == -1)
            {
                if (url.IndexOf("?") == -1)
                    return url + "?" + flag + "=1";
                else
                    return url + "&" + flag + "=1";
            }
            return url;
        }

        private void EnsureFlag(RouteValueDictionary routeValues, bool applyFlag, string flag)
        {
            if (applyFlag && !routeValues.Keys.Contains(flag))
                routeValues.Add(flag, 1);
        }


        private bool IsCurrentRequestRunInWindow
        {
            get
            {
                return Request.QueryString.AllKeys.Contains("iswindow");
            }
        }


        protected JsonResult Json(object data, object model)
        {
            ViewData.Model = model;
            return base.Json(data);
        }


        protected internal virtual ModelPermissionLimiters GetModelLimiterFunctions(dynamic model)
        {
            //TODO : make some conventions on it (for example : UserName,CreatorOwner,CreatedByUserName,OwnerUserName,CreatorUserName are good to automatically undrestand)
            //var modelType = (model as object).GetType();
            //if (modelType.prop .UserName != null)
            //    return model.UserName;
            //else if (model.CreatedByUserName != null)
            //    return model.CreatedByUserName;

            return null;
        }

        protected internal virtual Dictionary<string, string> GetLogDetails(int logPointId, dynamic model)
        {
            return null;
        }

        protected void Log<T>(T logId, object model, string[] exludeModelProperties = null) where T : struct
        {
            Logger.Current.Log(logId, model, exludeModelProperties);
        }

        public ContextManagerBase ContextManagerBase
        {
            get;
            protected set;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The main related Entity the current controller should work on</typeparam>
    /// <typeparam name="Y">The main Context Manager</typeparam>
    /// <typeparam name="Z">UnitOfRepository Interface class</typeparam>
    public abstract class CustomizedControllerBase<T, Y, Z> : CustomizedControllerBase
        where T : class ,new()
        where Y : ContextManagerBase, new()
    {
        protected Y _contextManager = null;
        public CustomizedControllerBase()
        {
            _contextManager = new Y();
            ContextManagerBase = _contextManager;
            _uoR = CreateUnitOfRepositoriesInstance();
        }

        private Z _uoR;
        protected Z UoR
        {
            get
            {
                return _uoR;
            }
        }

        protected abstract Z CreateUnitOfRepositoriesInstance();

    }


    public class RedirectToRouteResultEx : RedirectToRouteResult
    {

        public RedirectToRouteResultEx(RouteValueDictionary values)
            : base(values)
        {
        }

        public RedirectToRouteResultEx(string routeName, RouteValueDictionary values)
            : base(routeName, values)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var destination = new StringBuilder();

            var helper = new UrlHelper(context.RequestContext);
            destination.Append(helper.RouteUrl(RouteName, RouteValues));

            //Add href fragment if set
            if (!string.IsNullOrEmpty(Fragment))
            {
                destination.AppendFormat("#{0}", Fragment);
            }

            context.HttpContext.Response.Redirect(destination.ToString(), false);
        }

        public string Fragment { get; set; }

    }

    public static class RedirectToRouteResultExtensions
    {
        public static RedirectToRouteResultEx AddFragment(this RedirectToRouteResult result, string fragment)
        {
            return new RedirectToRouteResultEx(result.RouteName, result.RouteValues)
            {
                Fragment = fragment
            };
        }

        public static string ToString(this RedirectToRouteResult result, ControllerContext context)
        {
            var helper = new UrlHelper(context.RequestContext);
            return helper.RouteUrl(result.RouteName, result.RouteValues);
        }

        public static bool IsSavedSuccessfullyRedirect(this RedirectToRouteResult result)
        {
            if (result.RouteValues.ContainsValue(CustomizedControllerBase.SavedSuccessfullyActionName))
                return true;
            return false;

        }

        public static bool IsSavedSuccessfullyRedirect(this RedirectToRouteResultEx result)
        {
            if (result.Fragment.Contains(CustomizedControllerBase.SavedSuccessfullyFramgment))
                return true;
            else if (result.RouteValues.ContainsValue(CustomizedControllerBase.SavedSuccessfullyActionName))
                return true;
            return false;
        }

        public static bool IsSavedSuccessfullyRedirect(this RedirectResult result)
        {
            if (result.Url.Contains(CustomizedControllerBase.SavedSuccessfullyFramgment) || result.Url.Contains(CustomizedControllerBase.SavedSuccessfullyActionName))
                return true;
            return false;

        }
    }


    public class ModelPermissionLimiters
    {
        public PermissionLimiterBase[] LimiterFunctions { get; set; }
    }
}