var gulp = require('gulp');

var concat = require('gulp-concat');
var sass = require('gulp-ruby-sass');
var uglify = require('gulp-uglify');
var clean = require('gulp-clean');

var _destination = {
    source: {
        base: '../Potato.Core/Interface/',
        assets: '../Potato.Core/Interface/assets/'
    },
    builds: {
        base: '../../builds/Debug/Packages/Myrcon.Potato.Core.2.0.0/lib/net40/Interface/',
        assets: '../../builds/Debug/Packages/Myrcon.Potato.Core.2.0.0/lib/net40/Interface/assets/'
    }
};

var js = {
    lodash: 'src/_protected/assets/bower_components/lodash/dist/lodash.min.js',

    angular: 'src/_protected/assets/bower_components/angular/angular.min.js',
    angular_route: 'src/_protected/assets/bower_components/angular-route/angular-route.min.js',
    angular_animate: 'src/_protected/assets/bower_components/angular-animate/angular-animate.min.js',
    angular_touch: 'src/_protected/assets/bower_components/angular-touch/angular-touch.min.js',

    moment: 'src/_protected/assets/bower_components/moment/min/moment.min.js',
    angular_moment: 'src/_protected/assets/bower_components/angular-moment/angular-moment.min.js',

    ng_infinite_scroll: 'src/_protected/assets/bower_components/ngInfiniteScroll/build/ng-infinite-scroll.min.js',
    angular_slugify: 'src/_protected/assets/bower_components/angular-slugify/angular-slugify.js',
    angular_local_storage: 'src/_protected/assets/bower_components/angular-local-storage/angular-local-storage.min.js',
    angular_bindonce: 'src/_protected/assets/bower_components/angular-bindonce/bindonce.min.js',

    angular_bootstrap: 'src/_protected/assets/bower_components/angular-bootstrap/ui-bootstrap.min.js',
    angular_bootstrap_templates: 'src/_protected/assets/bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js'
};

var paths = {
    documents: [
        'src/*.html'
    ],
    fonts: [
        'src/_protected/assets/bower_components/bootstrap-sass-official/vendor/assets/fonts/bootstrap/**/*',
        'src/_protected/assets/bower_components/font-awesome/fonts/**/*'
    ],
    img: [
        'src/_protected/assets/img/**/*.png',
        'src/_protected/assets/img/**/*.gif',
        'src/_protected/assets/img/**/*.jpg',
        'src/_protected/assets/img/**/*.jpeg',
        'src/_protected/assets/img/**/*.ico'
    ],
    scss: [
        'src/_protected/assets/css/sass/**/*.scss'
    ],
    css: {
        user: [
            'src/_protected/assets/css/sass/style.scss'
        ],
        libs: [
            'src/_protected/assets/bower_components/animate.css/animate.css'
        ],
        built: [
            'src/_build/assets/css/**/*.css'
        ]
    },
    scripts: {
        compiled: [
            js.lodash,

            js.angular,
            js.angular_route,
            js.angular_animate,
            js.angular_touch,

            js.angular_bootstrap,
            js.angular_bootstrap_templates,

            js.moment,
            js.angular_moment,

            js.ng_infinite_scroll,
            js.angular_local_storage,
            js.angular_bindonce
        ],
        uncompiled: [,
            js.angular_slugify
        ],
        user: [
            'src/_protected/assets/js/**/*.js'
        ],
        built: [
            'src/_build/assets/js/main/compiled.min.js',
            'src/_build/assets/js/main/uncompiled.min.js',
            'src/_build/assets/js/main/user.min.js'
        ]
    }
};

// Bleach

gulp.task('bleach-build', function() {
    return gulp.src('src/_build', { read: false })
        .pipe(clean());
});


gulp.task('bleach-public', function() {
    return gulp.src('src/_public', { read: false })
        .pipe(clean());
});

gulp.task('bleach', ['bleach-build', 'bleach-public']);

// Javascript

gulp.task('js-compiled', [], function() {
    return gulp.src(paths.scripts.compiled)
        .pipe(concat('compiled.min.js'))
        .pipe(gulp.dest('src/_build/assets/js/main'));
});

gulp.task('js-uncompiled', [], function() {
    return gulp.src(paths.scripts.uncompiled)
        //.pipe(uglify())
        .pipe(concat('uncompiled.min.js'))
        .pipe(gulp.dest('src/_build/assets/js/main'));
});

gulp.task('js-user', function() {
    return gulp.src(paths.scripts.user)
        //.pipe(uglify())
        .pipe(concat('user.min.js'))
        .pipe(gulp.dest('src/_build/assets/js/main'));
});

gulp.task('js', ['js-compiled', 'js-uncompiled', 'js-user'], function() {
    return gulp.src(paths.scripts.built)
        .pipe(concat('script.min.js'))
        .pipe(gulp.dest(_destination.source.assets + 'js'))
        .pipe(gulp.dest(_destination.builds.assets + 'js'));
});

// Style sheets - Peeler

gulp.task('css-user', function() {
    return gulp.src(paths.css.user)
        .pipe(sass({ style: 'compressed', container: 'gulp-ruby-sass-css-user' }))
        .pipe(gulp.dest('src/_build/assets/css'));
});

gulp.task('css-libs', function() {
    return gulp.src(paths.css.libs)
        .pipe(gulp.dest('src/_build/assets/css'));
});

gulp.task('css', ['css-user', 'css-libs'], function() {
    return gulp.src(paths.css.built)
        .pipe(concat('style.min.css'))
        .pipe(gulp.dest(_destination.source.assets + 'css'))
        .pipe(gulp.dest(_destination.builds.assets + 'css'));
});

// Images
gulp.task('img', function() {
    return gulp.src(paths.img)
        .pipe(gulp.dest(_destination.source.assets + 'img'))
        .pipe(gulp.dest(_destination.builds.assets + 'img'));
});

// Fonts
gulp.task('fonts', function() {
    return gulp.src(paths.fonts)
        .pipe(gulp.dest(_destination.source.assets + 'fonts'))
        .pipe(gulp.dest(_destination.builds.assets + 'fonts'));
});

// Documents
gulp.task('documents', function() {
    return gulp.src(paths.documents)
        .pipe(gulp.dest(_destination.source.base))
        .pipe(gulp.dest(_destination.builds.base));
});

gulp.task('default', ['css', 'js', 'img', 'fonts', 'documents'], function() {
    return gulp.src('src/_build', { read: false })
        .pipe(clean());
});

gulp.task('watch', function () {
    gulp.watch(paths.documents, [ 'documents' ]);
    gulp.watch(paths.scripts.user, [ 'js' ]);
    gulp.watch(paths.scss, [ 'css' ]);
});