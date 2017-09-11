(function () {
    var domCache = {
        apiKeyInput: $('#input_apiKey'),
        exploreButton: $('#explore')
    };

    function addBearerTokenAuthorization() {
        var key = domCache.apiKeyInput.val();

        var apiKeyAuth = new SwaggerClient.ApiKeyAuthorization("Authorization", "Bearer " + key, "header");
        window.swaggerUi.api.clientAuthorizations.add("bearer", apiKeyAuth);
        window.swaggerUi.api.clientAuthorizations.remove("key");
        domCache.apiKeyInput.attr('placeholder', 'Put bearer token here');
    }

    addBearerTokenAuthorization();

    domCache.exploreButton.click(function() {
        addBearerTokenAuthorization();
    });
})();