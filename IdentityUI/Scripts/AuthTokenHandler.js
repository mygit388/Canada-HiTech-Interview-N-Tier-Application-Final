function storeTokens(refreshToken, expiresIn) {
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('tokenExpiry', Date.now() + (expiresIn * 1000));
}
/*function isTokenExpired() {
    const tokenExpiry = localStorage.getItem('tokenExpiry');
    if (!tokenExpiry || isNaN(tokenExpiry)) {
        return true;
    }
    var d = Date.now();
    var t = parseInt(tokenExpiry, 10);
    if (Date.now() > parseInt(tokenExpiry, 10))
        {
        return true;
    }
    else {
        return false;

    }
}
document.addEventListener("DOMContentLoaded", function () {

    const refreshToken = localStorage.getItem('refreshToken');
    if (refreshToken && isTokenExpired()) {
        const url = "https://localhost:44361/Account/RefreshToken";
        console.log('URL:', url); // Log the URL to the console

        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ refreshToken: refreshToken })
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json(); // Parse the response body as JSON
            })
            .then(data => {
                console.log('Response data:', data); // Log the response data

                if (data.Success) {
                   // console.log('AccessToken:', data.accessToken); // Log the access token
                    console.log('refreshToken:', data.refreshToken); // Log the refresh token

                    // Store the new tokens and expiry time
                    storeTokens(data.refreshToken, Date.now() + 120 * 1000 );

                    location.reload(); // Reload the page after storing tokens
                } else {
                    console.error('Failed to refresh token:', data.message);
                    // Handle token refresh failure (e.g., redirect to login page)
                }
            })
            .catch(error => {
                console.error('Error:', error);
                // Handle error (e.g., redirect to login page)
            });
    }
    
});*/
