angular.module('Potato.sandbox.program', [
    'ngRoute',
// Potato.Sandbox contains a few angular overrides
// so your script can function within the sandbox.
    'Potato.sandbox',
// Potato.Sandbox.Shared provides functionality to communicate between the main window and the iframe
// the plugin displays in
    'Potato.sandbox.shared'
])
.run(['$rootScope', 'MessageClient', function ($rootScope, MessageClient) {
    console.log('Starting Potato.sandbox.program..');

    // BEGIN IGNORE
    // I'll think of a better way of handling this at not-2am

    $rootScope.$on('$viewContentLoaded', function () {
        MessageClient.emit('sandbox.set-height', {
            Height: $('body').height()
        });
    });

    // END IGNORE
} ])
.config(['$routeProvider', function ($routeProvider) {
    // All of your templates must be strings or loaded from the $templateCache.
    // If you try to fetch a template externally it will fail.
    $routeProvider.when('/', {
        template: function () {
            return 'suuppp';
        },
        controller: 'SandboxTestController'
    })
    .when('/', { templateUrl: '/index.html', controller: 'IndexController' })
    .when('/widget/overview', { templateUrl: '/widget/overview.html', controller: 'WidgetOverviewController' })
    .when('/widget/settings', { templateUrl: '/widget/settings.html', controller: 'WidgetSettingsController' })
    .when('/widget/player', { templateUrl: '/widget/player.html', controller: 'WidgetPlayerController' })
    .when('/about', { templateUrl: '/about.html', controller: 'AboutController' })
    .otherwise({
        redirectTo: '/'
    });
} ])

// Much easier than inline messy objects. You can see Potato.Core doing something similar in it's own CommandBuilder.
.service('CommandBuilder', function () {
    // Builds the required data/command to send to our plugin for this command.
    this.TestPluginSimpleMultiplyByTwoCommand = function (number) {
        return {
            Command: 'TestPluginSimpleMultiplyByTwoCommand',
            // If you require a complex object to be passed through (Like a player) then you'll
            // need to be much more specific with this formatting.
            // If it's only a string/number/etc then the UI will format it for you in the "Content" List<String>
            Parameters: [number]
        };
    };
})

// I find it easier to split all this out into it's own service.
// This could included in the controller.
.service('MathematicsActions', ['MessageClient', 'CommandBuilder', function (MessageClient, CommandBuilder) {
    var $this = this;

    this.Communicating = false;

    this.Multiply = function (number) {
        $this.Communicating = true;

        return MessageClient.emit('plugin.command', CommandBuilder.TestPluginSimpleMultiplyByTwoCommand(number)).then(function (request) {
            console.log('whoo responseYEAH!', request);

            $this.Communicating = false;

            return request;
        }, function (error) {
            console.log('ohhh no an error!', error);

            $this.Communicating = false;

            return error;
        });
    };
} ])

.controller('MenuController', ['$scope', '$location', function ($scope, $location) {
    $scope.isActive = function(viewLocation) {
        var active = (viewLocation === $location.path());
        return active;
    };

} ])

.controller('IndexController', ['$scope', 'MathematicsActions', function ($scope, MathematicsActions) {
    $scope.SomeData = 'Some magical data!';

    // The object we're building up in the form.
    $scope.Mathematics = {
        Number: null,
        Result: 0
    };

    // We could just call MathematicsActions.Multiply from the form, but we want the callback to update our own scopes value.
    $scope.Submit = function () {
        // Using $q promise. It's in Angular's documentation.
        MathematicsActions.Multiply($scope.Mathematics.Number).then(function (request) {
            $scope.Mathematics.Result = request.Data.Message;
        });
    };

    $scope.MathematicsActions = MathematicsActions;


} ])
.controller('AboutController', [function () {

} ])
.controller('WidgetOverviewController', [function () {

} ])
.controller('WidgetSettingsController', [function () {

} ])
.controller('WidgetPlayerController', [function () {

} ]);