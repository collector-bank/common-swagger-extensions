(function () {
    var domCache = {
        apiKeyInput: $('#input_apiKey'),
        exploreButton: $('#explore')
    };

    function addBearerTokenAuthorization() {
        var key = domCache.apiKeyInput.val();

        var bearerTokenAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + key, "header");
        window.swaggerUi.api.clientAuthorizations.add("bearer", bearerTokenAuth);
        window.swaggerUi.api.clientAuthorizations.remove('api_key');
        domCache.apiKeyInput.attr('placeholder', 'Put bearer token here');
    }

    addBearerTokenAuthorization();

    domCache.apiKeyInput.change(addBearerTokenAuthorization);
})();