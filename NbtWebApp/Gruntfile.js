/*global module:false*/
module.exports = function (grunt) {
    var cswAppJsFiles = ['app/ChemSW.js', 'app/types/**.js', 'app/core/**.js', 'app/events/**.js', 'app/tools/**.js', 'app/literals/**.js', 'app/composites/**.js',
        'app/actions/**.js', 'app/layouts/**.js', 'app/nodes/**.js', 'app/pagecmp/**.js', 'app/proptypes/**.js', 'app/view/**.js',
        'app/wizards/**.js', 'app/Main.js'
    ]; //Unfortunately, due to race conditions in loading, we can't blindly accept all (YET)
    var cswAppCssFiles = ['css/ChemSW.css', 'css/csw*.css'];

    var cswTestJsFiles = ['test/*.js', 'test/*/*.js'];

    var cswVendorJsMinFiles = ['Scripts/modernizr-2.6.2.js', 'vendor/loggly-0.1.0.min.js', 'Scripts/es5-shim.min.js', 'vendor/es5-sham.min.js', 'vendor/es6-shim.min.js',
        'Scripts/mousetrap.min.js', 'Scripts/jquery-2.0.2.min.js', 'Scripts/jquery-ui-1.10.3.min.js', 'Scripts/jquery.validate.min.js',
        'Scripts/additional-methods.min.js', 'Scripts/jquery.cookie.js', 'vendor/jsTree.v.1.0rc2/jquery.jstree.js', 'vendor/jquery.multiselect.min.js',
        'vendor/jquery.multiselect.filter.js', 'vendor/jQueryFileUpload_2011.12.15/jquery.fileupload.js',
        'vendor/jQueryFileUpload_2011.12.15/jquery.iframe-transport.js', 'vendor/ext-init.js', 'vendor/extjs-4.1.0/ext-all.js', 'vendor/ext-done.js', 'vendor/jquery.flot.js',
        'vendor/extjs-4.1.0/ux/**/*.js'
    ];
    var cswVendorJsFiles = ['Scripts/modernizr-2.6.2.js', 'vendor/loggly-0.1.0.js', 'Scripts/es5-shim.js', 'vendor/es5-sham.min.js', 'vendor/es6-shim.js',
        'Scripts/mousetrap.js', 'Scripts/jquery-2.0.2.js', 'Scripts/jquery-ui-1.10.3.js', 'Scripts/jquery.validate.js',
        'Scripts/additional-methods.js', 'Scripts/jquery.cookie.js', 'vendor/jsTree.v.1.0rc2/jquery.jstree.js', 'vendor/jquery.multiselect.js',
        'vendor/jquery.multiselect.filter.js', 'vendor/jQueryFileUpload_2011.12.15/jquery.fileupload.js',
        'vendor/jQueryFileUpload_2011.12.15/jquery.iframe-transport.js', 'vendor/ext-init.js', 'vendor/extjs-4.1.0/ext-all-debug.js', 'vendor/ext-done.js', 'vendor/jquery.flot.js',
        'vendor/extjs-4.1.0/ux/**/*.js'
    ];

    var imageResources = ['Images/**/*.png', 'Images/**/*.gif', 'Images/**/*.jpg', 'Images/**/*.jpeg', 'vendor/**/*.png', 'vendor/**/*.gif', 'vendor/**/*.jpg', 'vendor/**/*.jpeg', 'Content/**/*.png', 'Content/**/*.gif', 'Content/**/*.jpg', 'Content/**/*.jpeg'];

    var cswVendorCssFiles = [];
    var buildDate = grunt.template.today("yyyy.m.d");
    var buildName = 'ChemSW.' + buildDate;
    // Project configuration.
    grunt.initConfig({
        CswAppJsFiles: cswAppJsFiles,
        CswAppCssFiles: cswAppCssFiles,

        CswTestJsFiles: cswTestJsFiles,
        cswVendorJsFiles: cswVendorJsFiles,

        imageResources: imageResources,

        pkg: grunt.file.readJSON('package.json'),

        meta: {
            //banner: '/*! <%= pkg.title || pkg.name %> - v<%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %>\n' +
            //    '<%= pkg.homepage ? "* " + pkg.homepage + "\n" : "" %>' + '* Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author.name %>\n */\n'
            banner: '/*! <%= pkg.title %> - v <%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %> */\n'
        },

        buildMode: 'prod',
        buildPrefix: 'release/' + buildName + '.min',

        clean: ['release'],

        concat: {
            login: { //ExternalLogin.html
                src: ['templates/MainHeader.txt', 'templates/MainForm_Start.txt', 'templates/LoginForm_Body.txt',
                      'templates/MainIncludes.txt', 'templates/LoginInit.txt', 'templates/MainEnd.txt'],
                dest: 'release/login.tmpl'
            },
            nodereport: { //NodeReport.html
                src: ['templates/MainHeader.txt', 'templates/NodeReport_Body.txt', 'templates/MainIncludes.txt', 'templates/NodeReportInit.txt', 'templates/MainEnd.txt'],
                dest: 'release/nodereport.tmpl',
            },
            report: { //Report.html
                src: ['templates/MainHeader.txt', 'templates/Report_Body.txt', 'templates/MainIncludes.txt', 'templates/ReportInit.txt', 'templates/MainEnd.txt'],
                dest: 'release/report.tmpl',
            },
            print: { //PrintingLabels.html
                src: ['templates/MainHeader.txt', 'templates/MainForm_Start.txt', 'templates/PrintForm_Body.txt', 'templates/MainForm_End.txt',
                     'templates/MainIncludes.txt', 'templates/MainEnd.txt'],
                dest: 'release/print.tmpl',
            },
            prod: { //Main.html
                src: ['templates/MainHeader.txt', 'templates/MainForm_Start.txt', 'templates/MainForm_Body.txt', 'templates/MainForm_End.txt',
                     'templates/MainIncludes.txt', 'templates/MainInit.txt', 'templates/MainEnd.txt'],
                dest: 'release/main.tmpl'
            },
            vendorCoreJs: {
                src: cswVendorJsMinFiles,
                dest: 'release/vendor.min.js',
                separator: ';/* next JS  */\n'
            },
            vendorCss: {
                src: cswVendorCssFiles,
                dest: 'release/vendor.min.css',
                separator: '/*  next CSS  */'
            },
            cswIntellisense: {
                src: cswAppJsFiles,
                dest: 'app/CswApp-vsdoc.js'
            }
        },

        cssmin: {
            options: {
                banner: '<%=meta.banner%>'
            },
            files: {
                src: cswAppCssFiles,
                dest: '<%= buildPrefix %>.css'
            }
        },

        docco: {
            src: cswAppJsFiles,
            options: {
                output: 'docs/'
            }

        },

        htmlminifier: {
            removeComments: true,
            collapseWhitespace: true,
            collapseBooleanAttributes: false,
            removeRedundantAttributes: false,
            removeEmptyAttributes: false,
            removeOptionalTags: false
        },

        jsdoc: {

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
                smarttabs: true,
                reporterOutput: 'release/jslint.txt',
                globals: {
                    $: true,
                    Csw: true,
                    window: true,
                    Ext: true
                }
            },
            globals: {
                $: true,
                Csw: true,
                window: true,
                Ext: true
            },
            files: cswAppJsFiles
        },

        lint: {
            files: cswAppJsFiles
        },

        min: {

        },

        plato: {
            test: {
                options: {
                    complexity: {
                        logicalor: false,
                        switchcase: false,
                        forin: true,
                        trycatch: true
                    }
                },
                globals: {
                    $: true,
                    Csw: true,
                    window: true,
                    Ext: true
                },
                files: {
                    'test/plato': cswAppJsFiles,
                },
            }
        },

        qunit: {
            files: ['test/*.html']
        },

        tmpHtml: {
            externalLogin: {
                src: 'templates/ExternalLoginIncludes.txt',
                dest: 'release/ExternalLoginIncludes.html'
            },
            login: {
                src: 'release/login.tmpl',
                dest: 'ExternalLogin.html',
            },
            nodereport: {
                src: 'release/nodereport.tmpl',
                dest: 'NodeReport.html',
            },
            report: {
                src: 'release/report.tmpl',
                dest: 'Report.html',
            },
            print: {
                src: 'release/print.tmpl',
                dest: 'PrintingLabels.html',
            },
            prod: {
                src: 'release/main.tmpl',
                dest: 'Main.html',
            },
            dev: {
                src: 'release/main.tmpl',
                dest: 'Dev.html',
            },
            pManifest: {
                src: 'templates/Manifest.txt',
                dest: 'release/Main.appcache'
            },
            dManifest: {
                src: 'templates/Manifest.txt',
                dest: 'release/Dev.appcache'
            },
            test: {
                src: 'test/test.tmpl',
                dest: 'test/Test.html',
                testJsFiles: cswTestJsFiles
            }
        },

        uglify: {
            options: {
                banner: '<%=meta.banner%>',
                ast: true,
                verbose: true,
                mangle: true,
                compress: true,
                //beautify: true,
                sourceMap: 'release/' + buildName + '.min.js.map',
                sourceMapRoot: '../',
                sourceMappingURL: buildName + '.min.js.map'
            },
            files: {
                src: cswAppJsFiles,
                dest: '<%= buildPrefix %>' + '.js'
            }
        },

        watch: {
            files: cswAppJsFiles,
            tasks: ['buildDev', 'runUnitTests', 'buildProd']
        }

    });

    /**REGION: *-contrib tasks */

    grunt.loadNpmTasks('grunt-contrib');
    grunt.loadNpmTasks('grunt-plato');

    /**ENDREGION: *-contrib tasks */

    /**REGION: init tasks */

    //grunt.loadNpmTasks('grunt-qunit-sonar');

    /**ENDREGION: init tasks */

    /**REGION: register CSW tasks */

    grunt.registerTask('buildProd', function () {
        grunt.task.run('clean'); //Delete anything in the 'release' folder
        grunt.task.run('cssmin'); //Compile the CSS
        grunt.task.run('concat'); //Assembles the HTML template
        grunt.task.run('uglify'); //Compile the JavaScript
        grunt.task.run('toHtml:prod:prod'); //Generate the Main HTML file from the template
        grunt.task.run('toHtml:prod:login'); //Generate the External Login HTML file from the template
        grunt.task.run('toHtml:prod:externalLogin'); //Generate the External Login Includes HTML file from the template
        grunt.task.run('toHtml:prod:nodereport'); //Generate the NodeReport HTML file from the template
        grunt.task.run('toHtml:prod:report'); //Generate the Report HTML file from the template
        grunt.task.run('toHtml:prod:print'); //Generate the PrintingLabels HTML file from the template
        grunt.task.run('toHtml:prod:pManifest'); //Generate the appcache from the manifest template
    });

    grunt.registerTask('buildDev', function (includeAllPages) {
        grunt.task.run('toHtml:dev:test'); //Generate the HTML file from the template
        grunt.task.run('toHtml:dev:dev'); //Generate the HTML file from the template
        if (includeAllPages) {
            grunt.task.run('toHtml:dev:login'); //Generate the External HTML file from the template
            grunt.task.run('toHtml:dev:nodereport'); //Generate the NodeReport HTML file from the template
            grunt.task.run('toHtml:dev:report'); //Generate the Report HTML file from the template
            grunt.task.run('toHtml:dev:print'); //Generate the PrintingLabels HTML file from the template
        }
        grunt.task.run('toHtml:dev:dManifest'); //Generate the appcache from the manifest template
        grunt.task.run('runUnitTests'); //Unit tests
    });

    grunt.registerTask('toHtml', function (buildMode, page) {
        //This needs to be a Grunt task, because we want it to run serially. If executed as a function, its sequence in the execution will be unknown
        grunt.log.write('Starting template-to-HTML conversion for ' + buildMode + ' mode.');
        if (buildMode === 'dev' || buildMode === 'prod') {
            grunt.config('buildMode', buildMode);
        }
        var conf = grunt.config('tmpHtml')[page];
        var tmpl = grunt.file.read(conf.src);

        grunt.file.write(conf.dest, grunt.template.process(tmpl));

        grunt.log.writeln('Generated \'' + conf.dest + '\' from \'' + conf.src + '\'');
    });

    grunt.registerTask('build', function (mode, target) {
        /// <summary>
        /// Plain vanilla build task, which supports two modes: dev and prod. Dev always builds prod. Syntax is `grunt.cmd build:dev`
        /// </summary>
        if (!mode) {
            throw grunt.task.taskError('Build mode must be supplied');
        }
        switch (mode) {
            case 'dev':
                grunt.task.run('buildProd');
                grunt.task.run('buildDev');
                break;
            case 'prod':
                grunt.task.run('buildProd');
                break;
        }
    });

    grunt.registerTask('runArbitraryTask', function (taskName, arg1, arg2) {
        /// <summary>
        /// Run any registered task
        /// </summary>
        if (!taskName) {
            throw grunt.task.taskError('Task Name must be supplied');
        }
        grunt.log.write(grunt.template.today('dddd mmmm dS yyyy h:MM:ss TT'));
        grunt.task.run(taskName, arg1, arg2);
    });

    grunt.registerTask('runUnitTests', function () {
        /// <summary>
        /// Build the Test HTML and execute the QUnit tests
        /// </summary>
        grunt.task.run('toHtml:dev:test'); //Generate the HTML file from the template
        grunt.task.run('jshint');
        grunt.task.run('qunit');
        grunt.task.run('plato');
    });

    /**REGION: register CSW tasks */
};
