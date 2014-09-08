'use strict';

angular.module('potato.data')

    .service('Instance', ['$interval', 'Communication', 'Connections', function($interval, Communication, Connections) {
        var $this = this;

        this.connections = Connections.create();

        this.refresh = function() {
            Communication.command('PotatoQuery').success(function(data) {
                $this.connections.parse(data.Now.Connections);
            });
        };

        this.dispose = function() {

        };

        $interval(this.refresh, 10000, 0, false);
        this.refresh();
    }])
;