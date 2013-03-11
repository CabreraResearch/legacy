/*global module:false*/
module.exports = function (grunt) {
    // Project configuration.
    grunt.initConfig({
        CswAppJsFiles: ['app/ChemSW.js', 'app/types/**.js', 'app/core/**.js', 'app/events/**.js', 'app/tools/**.js', 'app/literals/**.js', 'app/controls/**.js',
                'app/composites/**.js', 'app/actions/**.js', 'app/layouts/**.js', 'app/nodes/**.js', 'app/pagecmp/**.js', 'app/proptypes/**.js', 'app/view/**.js',
                'app/wizard/**.js'
        ], //Don't include 'Main.js' just yet. Unfortunately, due to racee conditions in loading, we can't blindly accept all (YET)
        CswTestJsFiles: ['test/*.js', 'test/**/*.js'],
        releaseMode: 'test',
        pkg: '<json:package.json>',
        meta: {
            banner: '/*! <%= pkg.title || pkg.name %> - v<%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %>\n' + '<%= pkg.homepage ? "* " + pkg.homepage + "\n" : "" %>' + '* No Copyright (!c) <%= grunt.template.today("yyyy") %> <%= pkg.author.name %>;' + ' Licensed <%= _.pluck(pkg.licenses, "type").join(", ") %>'
        },
        tmpHtml: {
            
            test: {
                src: 'test/test.tmpl',
                dest: 'test/Test.html',
                testJsFiles: ['test/*.js', 'test/**/*.js']
            }
        },
        concat: {
            
            vendorCoreJs: {
                src: ['Scripts/modernizr-2.6.2.js', 'vendor/loggly-0.1.0.min.js', 'Scripts/es5-shim.min.js', 'vendor/es5-sham.min.js', 'vendor/es6-shim.min.js', 'Scripts/mousetrap.min.js', 'Scripts/jquery-1.7.1.min.js',
                    'Scripts/jquery-ui-1.8.19.min.js', 'Scripts/jquery.validate.min.js', 'Scripts/additional-methods.min.js', 'Scripts/jquery.cookie.js', 'vendor/jsTree.v.1.0rc2/jquery.jstree.js',
                    'vendor/multiselect/jquery.multiselect.min.js', 'vendor/multiselect/jquery.multiselect.filter.min.js', 'vendor/jQueryFileUpload_2011.12.15/jquery.fileupload.js',
                    'vendor/jQueryFileUpload_2011.12.15/jquery.iframe-transport.js', 'vendor/ext-init.js', 'vendor/extjs-4.1.0/ext-all.js', 'vendor/ext-done.js'
                ],
                dest: 'vendor/vendor.min.js',
                separator: ';/* next JS  */\n'
            },
            vendorCss: {
                src: ['Content/themes/cupertino/jquery-ui-1.8.19.css', 'vendor/multiselect/jquery.multiselect.css', 'vendor/multiselect/jquery.multiselect.filter.css', 'vendor/extjs-4.1.0/resources/css/ext-all-scoped.css',
                      'vendor/extjs-4.1.0/ux/TabScrollerMenu.css', 'vendor/extjs-4.1.0/ux/css/CheckHeader.css'
                ],
                dest: 'vendor/vendor.min.css',
                separator: '/*  next CSS  */'
            }
        },
        min: {
            
        },
        qunit: {
            files: ['test/*.html']
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
    grunt.loadNpmTasks('grunt-contrib-jsdoc');

    /**ENDREGION: *-contrib tasks */

    /**REGION: init tasks */

    grunt.loadNpmTasks('grunt-closure-compiler');
    grunt.loadNpmTasks('grunt-jsdoc-plugin');

    //grunt.loadNpmTasks('grunt-qunit-sonar');

    /**ENDREGION: init tasks */

    /**REGION: register CSW tasks */

    var _buildHtmlFromTmpl = function (releaseMode, gruntConfigOpt) {
        grunt.log.write('Starting template-to-HTML conversion for ' + releaseMode + ' mode.');
        grunt.config('releaseMode', releaseMode);
        var conf = grunt.config(gruntConfigOpt);
        var tmpl = grunt.file.read(conf.src);

        grunt.file.write(conf.dest, grunt.template.process(tmpl));

        grunt.log.writeln('Generated \'' + conf.dest + '\' from \'' + conf.src + '\'');
    };

    grunt.registerTask('testTmpl', 'Generate Test/test.html from test.tmpl template.', function () {
        //Strictly speaking, it's not required to subtask these helper functions, but's it's easier to grok later --vlad
        _buildHtmlFromTmpl('test', 'tmpHtml.test');
    });

    grunt.registerTask('test', ['concat', 'jshint', 'concat', 'testTmpl', 'qunit']);

    /**REGION: register CSW tasks */
};
