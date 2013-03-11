/*global module:false*/
module.exports = function (grunt) {
    // Project configuration.
    grunt.initConfig({
        releasePath: 'release',
        pkg: '<json:package.json>',
        meta: {
            banner: '/*! <%= pkg.title || pkg.name %> - v<%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %>\n' + '<%= pkg.homepage ? "* " + pkg.homepage + "\n" : "" %>' + '* No Copyright (!c) <%= grunt.template.today("yyyy") %> <%= pkg.author.name %>;' + ' Licensed <%= _.pluck(pkg.licenses, "type").join(", ") %>'
        },
        tmpHtml: {
            release: {
                src: '',
                dest: ''
            },
            dev: {
                src: '',
                dest: ''
            }
        },
        concat: {
            dist: {
                src: '',
                dest: ''
            },
            vendorCoreJs: {
                src: [],
                dest: '',
                separator: ';\n\n\n;'
            },
            vendorCss: {
                src: [],
                dest: ''
            }
        },
        min: {
            dist: {
                src: [],
                dest: '',
                separator: ';\n\n\n;'
            }
        },
        qunit: {
            files: []
        },
        lint: {
            files: []
        },
        watch: {
            files: [],
            tasks: ''
        },
        jshint: {
            options: {
                bitwise: true,
                curly: true,
                eqeqeq: true,
                forin: true,
                immed: true,
                latedef: true,
                newcap: true,
                noarg: true,
                plusplus: true,
                sub: true,
                undef: true,
                boss: true,
                eqnull: true,
                strict: false,
                browser: true,
                globalstrict: false,
                smarttabs: true
            },
            globals: {
                $: true,
                Csw: true,
                window: true,
                Ext: true
            }
        },
        uglify: {
            ast: true,
            verbose: true
        },
        'closure-compiler': {

        },
        'append-sourcemapping': {

        },
        jsdoc: {

        }
    });

    /**REGION: *-contrib tasks */

    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-qunit');
    grunt.loadNpmTasks('grunt-contrib-connect');
    grunt.loadNpmTasks('grunt-contrib-watch');

    /**ENDREGION: *-contrib tasks */

    /**REGION: init tasks */

    grunt.loadNpmTasks('grunt-closure-compiler');
    grunt.loadNpmTasks('grunt-jsdoc-plugin');

    //grunt.loadNpmTasks('grunt-qunit-sonar');

    /**ENDREGION: init tasks */

    /**REGION: register tasks */


    /**REGION: register tasks */
};
