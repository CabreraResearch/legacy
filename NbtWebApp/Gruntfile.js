/*global module:false*/
module.exports = function (grunt) {
    var cswAppJsFiles = ['app/ChemSW.js', 'app/types/**.js', 'app/core/**.js', 'app/events/**.js', 'app/tools/**.js', 'app/literals/**.js', 'app/controls/**.js',
        'app/composites/**.js', 'app/actions/**.js', 'app/layouts/**.js', 'app/nodes/**.js', 'app/pagecmp/**.js', 'app/proptypes/**.js', 'app/view/**.js',
        'app/wizards/**.js', 'app/Main.js'
    ]; //Unfortunately, due to race conditions in loading, we can't blindly accept all (YET)
    var cswAppCssFiles = ['css/ChemSW.css', 'css/csw*.css'];

    var cswTestJsFiles = ['test/*.js', 'test/*/*.js'];

    var cswVendorJsFiles = ['Scripts/modernizr-2.6.2.js', 'vendor/loggly-0.1.0.min.js', 'Scripts/es5-shim.min.js', 'vendor/es5-sham.min.js', 'vendor/es6-shim.min.js',
        'Scripts/mousetrap.min.js', 'Scripts/jquery-1.9.1.min.js', 'Scripts/jquery-ui-1.10.1.min.js', 'Scripts/jquery.validate.min.js',
        'Scripts/additional-methods.min.js', 'Scripts/jquery.cookie.js', 'vendor/jsTree.v.1.0rc2/jquery.jstree.js', 'vendor/multiselect/jquery.multiselect.min.js', 
        'vendor/multiselect/jquery.multiselect.filter.min.js', 'vendor/jQueryFileUpload_2011.12.15/jquery.fileupload.js',
        'vendor/jQueryFileUpload_2011.12.15/jquery.iframe-transport.js', 'vendor/ext-init.js', 'vendor/extjs-4.1.0/ext-all.js', 'vendor/ext-done.js'
    ];
    var cswVendorCssFiles = [];    

    // Project configuration.
    grunt.initConfig({
        CswAppJsFiles: cswAppJsFiles,
        CswAppCssFiles: cswAppCssFiles,
        
        CswTestJsFiles: cswTestJsFiles,

        pkg: grunt.file.readJSON('package.json'),

        meta: {
            //banner: '/*! <%= pkg.title || pkg.name %> - v<%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %>\n' +
            //    '<%= pkg.homepage ? "* " + pkg.homepage + "\n" : "" %>' + '* Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author.name %>\n */\n'
            banner: '/*! <%= pkg.title %> - v <%= pkg.version %> - ' + '<%= grunt.template.today("yyyy-mm-dd") %> */\n'
        },

        buildMode: 'prod',
        buildPrefix: 'release/ChemSW.' + grunt.template.today("yyyy.m.d") + '.min',

        clean: ['release'],

        concat: {
            login: { //ExternalLogin.html
                src: ['templates/MainHeader.txt', 'templates/MainForm_Start.txt', 'templates/LoginForm_Body.txt', 'templates/MainForm_End.txt',
                      'templates/MainIncludes.txt', 'templates/LoginFormInit.txt', 'templates/MainEnd.txt'],
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
                src: cswVendorJsFiles,
                dest: 'vendor/vendor.min.js',
                separator: ';/* next JS  */\n'
            },
            vendorCss: {
                src: cswVendorCssFiles,
                dest: 'vendor/vendor.min.css',
                separator: '/*  next CSS  */'
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
                smarttabs: true
            },
            globals: {
                $: true,
                Csw: true,
                window: true,
                Ext: true
            }
        },
        
        lint: {
            files: cswAppJsFiles
        },
        
        min: {

        },
        
        qunit: {
            files: ['test/*.html']
        },

        tmpHtml: {
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
                sourceMap: '<%= buildPrefix %>' + '.map'
            },
            files: {
                src: cswAppJsFiles,
                dest: '<%= buildPrefix %>' + '.js'
            }
        },
        
        watch: {
            files: cswAppJsFiles,
            tasks: 'lint qunit'
        }
        
    });

    /**REGION: *-contrib tasks */

    grunt.loadNpmTasks('grunt-contrib');

    /**ENDREGION: *-contrib tasks */

    /**REGION: init tasks */
    
    //grunt.loadNpmTasks('grunt-qunit-sonar');

    /**ENDREGION: init tasks */

    /**REGION: register CSW tasks */

    grunt.registerTask('buildProd', function () {
        grunt.task.run('clean'); //Delete anything in the 'release' folder
        grunt.task.run('cssmin'); //Compile the CSS
        grunt.task.run('concat'); ////Assembles the HTML template
        grunt.task.run('uglify'); //Compile the JavaScript
        grunt.task.run('toHtml:prod'); //Generate the Main HTML file from the template
        grunt.task.run('toHtml:login'); //Generate the External HTML file from the template
        grunt.task.run('toHtml:nodereport'); //Generate the NodeReport HTML file from the template
        grunt.task.run('toHtml:report'); //Generate the Report HTML file from the template
        grunt.task.run('toHtml:print'); //Generate the PrintingLabels HTML file from the template
    });

    grunt.registerTask('toHtml', function (buildMode) {
        //This needs to be a Grunt task, because we want it to run serially. If executed as a function, its sequence in the execution will be unknown
        grunt.log.write('Starting template-to-HTML conversion for ' + buildMode + ' mode.');
        if (buildMode === 'dev' || buildMode === 'prod') {
            grunt.config('buildMode', buildMode);
        }
        var conf = grunt.config('tmpHtml')[buildMode];
        var tmpl = grunt.file.read(conf.src);

        grunt.file.write(conf.dest, grunt.template.process(tmpl));

        grunt.log.writeln('Generated \'' + conf.dest + '\' from \'' + conf.src + '\'');
    });
    
    grunt.registerTask('build', function(mode, target) {
        /// <summary>
        /// Plain vanilla build task, which supports two modes: dev and prod. Dev always builds prod. Syntax is `grunt.cmd build:dev`
        /// </summary>
        if (!mode) {
            throw grunt.task.taskError('Build mode must be supplied');
        }
        switch(mode) {
            case 'dev':
                grunt.task.run('buildProd');
                grunt.task.run('toHtml:test'); //Generate the HTML file from the template
                grunt.task.run('toHtml:dev'); //Generate the HTML file from the template
                grunt.task.run('qunit'); //Unit tests
                break;
            case 'prod':
                grunt.task.run('buildProd'); 
                break;
        }
    });

    grunt.registerTask('runArbitraryTask', function (taskName) {
        /// <summary>
        /// Run any registered task
        /// </summary>
        if (!taskName) {
            throw grunt.task.taskError('Task Name must be supplied');
        }
        grunt.task.run(taskName);
    });

    /**REGION: register CSW tasks */
};
