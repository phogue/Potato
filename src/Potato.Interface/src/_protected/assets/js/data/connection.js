'use strict';

angular.module('potato.data')

    .factory('Connection', ['$cacheFactory', '$interval', 'Communication', function($cacheFactory, $interval, Communication) {
        var cache = $cacheFactory('connections');

        var Connection = function(attributes) {
            var $this = this;

            // A server-side generated identifier for this connection.
            this.id = null;

            angular.extend(this, attributes);

            var intervalPromise = $interval(this.refresh, 10000, 0, false);
            this.refresh();

            // Remove all listeners and data for this object
            // Close the current connection, cleaning up the data within this object
            this.dispose = function() {
                cache.remove(this.id);

                if (intervalPromise) {
                    $interval.cancel(intervalPromise);
                    intervalPromise = null;
                }
            }
        };

        angular.extend(Connection.prototype, {
            refresh: function() {
                var $this = this;

                Communication.command('ConnectionQuery', [ ], {
                    ConnectionGuid: this.ConnectionGuid
                }).success(function(data) {
                    angular.extend($this, data.Now);
                });
            }
        });

        var factory = {
            key: function(item) {
                return item.ConnectionGuid;
            },

            // Pull a connection from cache
            getCache: function(attributes) {
                var connection = null;

                // Check if we have an existing object in cache
                if (attributes) {
                    connection = cache.get(factory.key(attributes));
                }

                return connection;
            },

            // Put a connection in cache, override anything currently set.
            putCache: function(connection) {
                if (connection) {
                    cache.put(factory.key(connection), connection);
                }
            },

            create: function(attributes) {
                var connection = factory.getCache(attributes);

                // If cache yielded nothing, create a new connection.
                if (!connection) {
                    connection = new Connection(attributes);

                    connection.id = factory.key(connection);

                    factory.putCache(connection);
                }

                return connection;
            }
        };

        return factory;
    }])
;