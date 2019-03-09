# Token Exchange Sample Service
This service demonstrates how to implement a token exchange endpoint 
using .NET MVC4. Token exchange is the process by which one application
(e.g. a custom tool/form/plugin) obtains an access token from another
application to which the user is already authenticated (e.g. LO Connect).

Token exchange follows the following flow:

1. The "guest" application (e.g. custom form) requests an **auth code**
from the "host" application (e.g. LO Connect). Typically, this will be done
using the Auth.getAuthCode() function exposed to the guest process via the
Ellie Mae JavaScript Framework.

2. The guest invokes a token excahnge endpoint on the guest's web servers, passing in
the auth code obtained from Step 1. 

3. The guest's web servers invoke the Ellie Mae Identity Service's token endpoint
(https://api.elliemae.com/v2/oauth/token), passing in the auth code along with the
guest's OAuth Client ID and Secret.

4. Ellie Mae's Identity Service validates that the auth code is valid and that
the specified OAuth Client information is authorized to exchange the auth code for an
access token. Assuming success, the Identity Service return an **access token** to 
the guest's web servers.

5. The guest's web servers, upon receipt of the access token, can store this token
(e.g. in a session variable) for future use or may return the token back to the client-
side script running in the browser. 

6. The guest script (and/or web servers) can now make API calls to Ellie Mae's REST APIs
by passing the access token within the Authorization header of the request.

The flow requires a server-side implementation to perform the exchange in order to 
maintain the secrecy of the guest's OAuth Secret. Ellie Mae's token exchange endpoint
should never be called from the browser as this would compromise the privacy of this
secret.

## Setting up the Sample
The sample project runs under .NET Framework 4.5.2, and can be hosted on any Windows system 
running IIS. Before running the sample, you will need to modify the web.config file to
replace the tokens "your_oauth_clientid_here" and "your_oauth_secret_here" with your
organization's OAuth credentials.

The sample service exposes a single endpoint: `/api/token`. The endpoint expects a POST call
with a body that contains a JSON document with the following format:

```javascript
{
  "authCode": "<auth_code_here>"
}
```

The <auth_code_here> value should be replaced by the value obtained by calling the 
Auth.getAuthCode() function in your JavaScript code, e.g.

```javascript
async function generateAccessToken() {

    let auth = await elli.script.getObject("auth");

    // Generate the auth code
    let authCode = await auth.createAuthCode();

	// Exchange for an access token by calling a custom API
	let resp = await $.ajax({
		method: "POST",
		url: "https://api.mysamplesite.com/tokenexchange/api/token",
		data: { authCode: authCode }
	});

	return resp.access_token;
}
```

Upon success, the API will return a JSON document with the access_token attribute
containing the access token that can be used for subsequent calls to Ellie Mae's
REST APIs.
