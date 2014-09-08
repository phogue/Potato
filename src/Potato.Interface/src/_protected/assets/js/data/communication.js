'use strict';

angular.module('potato.data')

    .service('Communication', ['_', '$http', function(_, $http) {

        this.command = function(name, parameters, scope) {
            return $http.post('/', {
                Name: name,
                Parameters: parameters || [ ],
                Scope: scope || { }
            });
        };

    }])
;