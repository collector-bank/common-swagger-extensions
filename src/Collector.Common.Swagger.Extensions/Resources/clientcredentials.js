(function () {
    var domCache = {
        apiKeyInput: $('#input_apiKey'),
        exploreButton: $('#explore')
    };

    function addApiKeyAuthorization() {
        var key = domCache.apiKeyInput.val();

        if (key && key.trim() != "") {
            var apiKeyAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + key, "header");
            window.swaggerUi.api.clientAuthorizations.add("bearer", apiKeyAuth);
            window.swaggerUi.api.clientAuthorizations.remove("api_key");
        }
    }

    domCache.apiKeyInput.change(addApiKeyAuthorization);
    domCache.apiKeyInput.attr('placeholder', 'Put bearer token here');
    domCache.exploreButton.hide();

    window.swaggerUi.api.clientAuthorizations.remove("api_key");
})();