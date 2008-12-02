﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Lephone.Data;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Util;

namespace Lephone.Web.Rails
{
    public class ControllerBase
    {
        protected internal HttpContext ctx;
        protected internal Dictionary<string, object> bag = new Dictionary<string, object>();

        protected internal virtual void OnBeforeAction(string ActionName)
        {
        }

        protected internal virtual void OnAfterAction(string ActionName)
        {
        }

        protected internal virtual void OnException(Exception ex)
        {
            Exception e = ex.InnerException ?? ex;
            ctx.Response.Write(string.Format("<h1>{0}<h1>", ctx.Server.HtmlEncode(e.Message)));
        }
    }

    [Scaffolding]
    public class ControllerBase<T> : ControllerBase where T : class, IDbObject
    {
        protected FlashBox flash = new FlashBox();

        protected void RedirectTo(UTArgs args, params object[] paramters)
        {
            if(string.IsNullOrEmpty(args.Controller))
            {
                args.Controller = GetControllerName();
            }
            string url = PageBase.UrlTo(ctx.Request.ApplicationPath, args.Controller, args.Action, paramters);
            ctx.Response.Redirect(url);
        }

        private string GetControllerName()
        {
            string ControllerName = GetType().Name;
            if (ControllerName.EndsWith("Controller"))
            {
                ControllerName = ControllerName.Substring(0, ControllerName.Length - 10);
            }
            return ControllerName.ToLower();
        }

        public virtual void New()
        {
        }

        public virtual void Create()
        {
            string ControllerName = GetControllerName();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = (T)oi.NewObject();
            foreach(MemberHandler m in oi.Fields)
            {
                if (!m.IsRelationField && !m.IsDbGenerate && !m.IsAutoSavedValue)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.IsLazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.FieldType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                    }
                }
            }
            DbEntry.Save(obj);
            flash["notice"] = string.Format("{0} was successfully created", ControllerName);
            RedirectTo(new UTArgs{Action = "list"});
        }

        public virtual void List(int PageIndex)
        {
            if (PageIndex < 0)
            {
                throw new DataException("The PageIndex out of supported range.");
            }
            if (PageIndex != 0)
            {
                PageIndex--;
            }
            IPagedSelector ps = DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy("Id DESC")
                .PageSize(WebSettings.DefaultPageSize).GetPagedSelector();
            bag["list"] = ps.GetCurrentPage(PageIndex);
            bag["list_count"] = ps.GetResultCount();
            bag["list_pagesize"] = WebSettings.DefaultPageSize;
        }

        public virtual void Show(int n)
        {
            bag["item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Edit(int n)
        {
            bag["item"] = DbEntry.GetObject<T>(n);
        }

        public virtual void Update(int n)
        {
            string ControllerName = GetControllerName();
            ObjectInfo oi = ObjectInfo.GetInstance(typeof(T));
            var obj = DbEntry.Context.GetObject(typeof (T), n);
            foreach (MemberHandler m in oi.Fields)
            {
                if (m.IsRelationField) { continue; }
                if (!m.IsAutoSavedValue && !m.IsDbGenerate)
                {
                    string s = ctx.Request.Form[ControllerName + "[" + m.Name.ToLower() + "]"];
                    if (m.IsLazyLoad)
                    {
                        object ll = m.MemberInfo.GetValue(obj);
                        PropertyInfo pi = m.MemberInfo.MemberType.GetProperty("Value");
                        object v = ControllerHelper.ChangeType(s, m.FieldType.GetGenericArguments()[0]);
                        pi.SetValue(ll, v, null);
                        // TODO: get rid of use such method.
                        typeof (T).GetMethod("m_ColumnUpdated", ClassHelper.AllFlag).Invoke(obj, new object[] {m.Name});
                    }
                    else
                    {
                        m.MemberInfo.SetValue(obj, ControllerHelper.ChangeType(s, m.FieldType));
                    }
                }
            }
            DbEntry.Save(obj);
            flash["notice"] = string.Format("{0} was successfully updated", ControllerName);
            RedirectTo(new UTArgs{Action = "show"}, n);
        }

        public virtual void Destroy(int n)
        {
            object o = DbEntry.GetObject<T>(n);
            if (o != null)
            {
                DbEntry.Delete(o);
                RedirectTo(new UTArgs{Action = "list"});
            }
        }
    }
}
