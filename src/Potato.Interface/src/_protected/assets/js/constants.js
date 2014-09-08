'use strict';

angular.module('potato')

    .constant('moment', window.moment)

    .constant('IndexedTeamColoursHex', {
        '1': '#ff464b',
        '2': '#2d5ff0',
        '3': '#99e899',
        '4': '#ffd48c'
    })

    .constant('LabelColoursHex', {
        EmptyBlurred: '#8b91a0',
        Empty: '#eeeeee',
        Info: '#5bc0de',
        Success: '#5cb85c',
        Warning: '#f0ad4e',
        Danger: '#d9534f'
    })
;