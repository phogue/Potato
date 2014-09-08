'use strict';

// Declare app level module which depends on filters, and services
angular.module('potato', [
        'ngRoute',
        'ngAnimate',
        'ngTouch',
        'ui.bootstrap',
        'angularMoment',
        'infinite-scroll',
        'slugifier',
        'LocalStorageModule',
        'pasvaz.bindonce',
        'potato.constants',
        'potato.data',
        'potato.directives',
        'potato.controllers'
    ])

    .config(['$routeProvider', function($routeProvider) {
        $routeProvider.when('/', {
            templateUrl: 'assets/partials/index.html',
            controller: 'IndexController'
        })
        .otherwise({ redirectTo: '/' });
    }])
;

angular.module('potato.constants', [])
    .constant('_', window._)

    .constant('moment', window.moment);

angular.module('potato.data', [
    'potato.constants'
]);

angular.module('potato.directives', [
    'potato.constants',
    'potato.data'
]);

angular.module('potato.controllers', [
    'potato.constants',
    'potato.data'
]);

