﻿using System;
using System.Web;
using CookieRestrictions.Configuration;

namespace CookieRestrictions
{
    public static class CookieUtil
    {
        public static void AllowCookies()
        {
            HttpCookie allowCookie = new HttpCookie(CookieRestrictionsConfig.Instance.CookiesAllowedKey, "on");
            allowCookie.Expires = DateTime.MaxValue;
            allowCookie.HttpOnly = false;
            HttpContext.Current.Response.Cookies.Add(allowCookie);
        }


        public static void DisallowCookies()
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(CookieRestrictionsConfig.Instance.CookiesAllowedKey);
            if(httpCookie == null)
                return;
            httpCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(httpCookie);
        }

        public static bool AllowingCookies()
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(CookieRestrictionsConfig.Instance.CookiesAllowedKey);
            if (httpCookie == null)
                return false;
            return httpCookie.Value == "on" ? true : false;
        }

    }
}