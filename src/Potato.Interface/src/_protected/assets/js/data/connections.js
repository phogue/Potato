'use strict';

angular.module('potato.data')

    .factory('Connections', ['Collection', 'Connection', function(Collection, Connection) {
        var Connections = function(attributes) {
            Collection.type.apply(this, arguments);

            this.model = Connection;
        };

        Connections.prototype = angular.extend(new Collection.type(), {

        });

        return {
            create: function (attributes) {
                return new Connections(attributes);
            }
        };
    }])
;