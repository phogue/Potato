'use strict';

angular.module('potato.data')

    .factory('Collection', [function() {
        var Collection = function(attributes) {
            // List of conversations attached to this instance
            this.items = [];

            this.model = null;

            angular.extend(this, attributes);
        };

        angular.extend(Collection.prototype, {

            // Parses the items creating models for each
            parse: function(items) {
                this.items = _.map(items, this.model.create);
            },

            // Grabs the key for a specifc
            key: function(item) {
                return this.model.key(item);
            },

            // Fetches an item from the list
            getById: function(id) {
                return _.find(this.items, {
                    id: id
                });
            },

            // Helper to determine if an item exists by id
            existsById: function(id) {
                return !!this.getById(id);
            },

            // Removes an item from the list by its id
            removeById: function(id) {
                _.remove(this.items, {
                    id: id
                });
            },

            // Dispose each item
            _disposeItems: function() {
                for (var offset = 0; offset < this.items.length; offset++) {
                    this.items[offset].dispose();
                }
            }
        });

        return {
            type: Collection
        };
    }])
;