'use strict';

angular.module('potato.controllers')

    .controller('IndexController', ['$scope', 'Instance', function($scope, Instance) {
        $scope.instance = Instance;

    }])
;