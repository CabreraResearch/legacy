/*global module:false*/
module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: '<json:CswApp.json>',
    meta: {
      banner: '/*! <%= pkg.title || pkg.name %> - v<%= pkg.version %> - ' +
        '<%= grunt.template.today("yyyy-mm-dd") %>\n' +
        '<%= pkg.homepage ? "* " + pkg.homepage + "\n" : "" %>' +
        '* Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author.name %>;' +
        ' Licensed <%= _.pluck(pkg.licenses, "type").join(", ") %> */',
	    ducksboard_api_key: 'iMPZSvSciCQFntmd9MPlPvcYpEV4rBOu1tAv2TJWgEGDLBDCKh'
    },
    concat: {
      dist: {
        src: ['app/ChemSW.js', 'app/**/*.js', 'app/Main.js'],
        dest: 'CswApp.<%= grunt.template.today("yyyy.m.d") %>.js'
      },
      html: {
        src: ['MainHead.html', 'MainIncludes.html', 'MainCswIncludes.html', 'MainHeader.html', 'Main.html', 'MainFooter.html'],
        dest: 'Index.html'
      }  
    },    
    min: {
      dist: {
        src: ['CswApp.<%= grunt.template.today("yyyy.m.d") %>.js'],
        dest: 'CswApp.<%= grunt.template.today("yyyy.m.d") %>.min.js',
        separator: ';'
      }
    },  
    qunit: { 
      files: ['test/**/*.html']
    },
    lint: {
      files: ['app/ChemSW.js', 'app/**/*.js', 'app/Main.js']
    },
    watch: {
      files: '<config:lint.files>',
      tasks: 'concat min closure-compiler'
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
        globalstrict: false
      },
      globals: {
        jQuery: true,
        ChemSW: true,
        Csw: true,
        '$': true,
        window: true,
        'Ext': true
      }
    },
    uglify: { 
      ast: true, 
      verbose: true
    },
    'closure-compiler': {
      frontend: {
        js: '<config:concat.dist.dest>',
        jsOutputFile: 'new.min.js',
        options: {
          language_in: 'ECMASCRIPT5_STRICT'
        }
      }
    },
    docco: {
      files: ['app/ChemSW.js', 'app/**/*.js']
    },
    ducksboard: {
          tasks: {
              src: ['tasks/*.js'],
              endpoint: 'iMPZSvSciCQFntmd9MPlPvcYpEV4rBOu1tAv2TJWgEGDLBDCKh'
          }
      }
  	
  });
 
  grunt.loadNpmTasks('grunt-barkeep');
  grunt.loadNpmTasks('grunt-growl');
  grunt.loadNpmTasks('grunt-closure-compiler');
  
  // Default task.
  grunt.registerTask('default', 'qunit concat closure-compiler watch');

  //grunt.loadNpmTasks('grunt-closure-compiler');
  grunt.registerTask('dev', 'concat watch qunit');
  
  
}; 
