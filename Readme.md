# 1508 CookieRestrictions
-------
A way of turning cookies on and off to support the regulative (EU 32002L0058) 

By adding the queryparameter allowCookies=on or disallowCookies=on cookies will be disabled or enabled.
(cookis are enabled by setting an allowCookies cookie with the value "on")
			
- On the server side a http-module attached to the ResponseEnd event will clear all cookies if they are not enabled
- On the client side a javascript will wait 1200 milliseconds before clearing all cookies which may have been set via 3rd party javascript includes (such as google analytics)			
- The HttpModule can be configured to work on a limited number of hostnames, just add them as a comma separated list in the app-setting "CookieRestrictions.ValidHostnames"
- The javascript is placed in \resources\js\CookieRestrictions.js and must be included manualy if needed

1508 / Design in Love with Technology