using System;
using System.Web;
using CookieRestrictions.Configuration;

namespace CookieRestrictions.HttpModules
{
    public class CookieRestrictionModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose() 
        {
            
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(context_EndRequest);            
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            if(HttpContext.Current == null || HttpContext.Current.Request == null)
            {
                return;
            }

            string hostname = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            if (string.IsNullOrEmpty(hostname) || (CookieRestrictionsConfig.Instance.ValidHostnames.Count > 0 && !CookieRestrictionsConfig.Instance.ValidHostnames.Contains(hostname)))
                return;

            // Get or Set the cookies allowed cookie
            bool disallowCookies = GetRequestVar(CookieRestrictionsConfig.Instance.CookiesNotAllowedkey) == "on";
            HttpCookie allowCookie = HttpContext.Current.Request.Cookies.Get(CookieRestrictionsConfig.Instance.CookiesAllowedKey);
            if (allowCookie == null && GetRequestVar(CookieRestrictionsConfig.Instance.CookiesAllowedKey) == "on" && !disallowCookies)
            {
                allowCookie = new HttpCookie(CookieRestrictionsConfig.Instance.CookiesAllowedKey, "on");
                allowCookie.Expires = DateTime.MaxValue;
                allowCookie.HttpOnly = false;
                HttpContext.Current.Response.Cookies.Add(allowCookie);
            }
            
            // Return if cookies are allowed
            if (allowCookie != null && allowCookie.Value == "on" && !disallowCookies)
                return;                        

            // Otherwise
            // Clear all cookies currently set
            HttpContext.Current.Response.Cookies.Clear();

            // Clear all existing cookies
            string[] allKeys = HttpContext.Current.Request.Cookies.AllKeys;
            string requestCookieHader = HttpContext.Current.Request.Headers["Cookie"];
            foreach (string key in allKeys)
            {
                // For some reason asp.net adds the asp.net session cookie to the request cookies even if the browser did not send any (this check ensures that the key has actualy been sent as part of the request header)
                if (!string.IsNullOrEmpty(requestCookieHader) && requestCookieHader.Contains(string.Concat(key, "=")))
                {
                    HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(key);
                    HttpCookie tmp = new HttpCookie(cookie.Name, string.Empty);
                    tmp.Expires = DateTime.Now.AddMinutes(-1);
                    tmp.Domain = cookie.Domain;
                    tmp.HttpOnly = cookie.HttpOnly;
                    HttpContext.Current.Response.Cookies.Add(tmp);
                }
            }
        }

        private string GetRequestVar(string key)
        {
            string val = HttpContext.Current.Request[key];
            if (string.IsNullOrEmpty(val))
                return string.Empty;

            return val;
        }

        #endregion
    }
}
