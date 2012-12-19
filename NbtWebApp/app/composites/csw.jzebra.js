/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.jZebra = Csw.composites.jZebra ||
        Csw.composites.register('jZebra', function (cswParent, options) {
            /// <summary>
            /// Create a jZebra applet and return the an object for use with printing
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.ID: An ID for the linkGrid.</para>
            /// <para>options.linkText: linkGrid link prefix</para>
            /// </param>
            /// <returns type="linkGrid">A linkGrid object</returns>
            'use strict';
            var cswPrivate = {
                name: 'jzebra',
                cssclass: '',
                dynamicallyInjectApplet: false
            };
            var cswPublic = {};

            Csw.tryExec(function () {
                //This is the 3rd party JS lib
                if (Csw.isNullOrEmpty(window.jZebra)) {
                    Csw.error.throwException('Cannot create a jZebra component without the jZebra library.', 'csw.jzebra.js', 'csw.jzebra.js', 27);
                }
                cswPublic.zebraJs = window.jZebra;

                (function _pre() {
                    Csw.extend(cswPrivate, options);

                    cswPrivate.div = cswParent.div();

                    //We don't have a need yet to dynamically inject a Java applet. When we do, this is what it would look like.

                    //if (cswPrivate.dynamicallyInjectApplet &&
                    //    Csw.isNullOrEmpty(document.jZebra)) {
                    //    //The 3rd party Java applet
                    //    cswPrivate.applet = cswPrivate.div.applet({
                    //        name: 'jZebra',
                    //        code: 'jzebra.PrintApplet.class',
                    //        archive: 'js/thirdparty/jZebra/jzebra.jar'
                    //    });
                    //} else {
                    //    cswPrivate.applet = cswPrivate.div.find('#jZebra');
                    //}

                    cswPublic.defaultPrinter = Csw.string(Csw.cookie.get('defaultPrinter'));

                    //To minimize hacking jZebra, use their ID
                    Csw.tryExec(function () {

                        cswPrivate.printerSel = cswPrivate.div.select({
                            cssclass: 'CswZebraPrintersList',
                            name: 'CswZebraPrintersList',
                            selected: cswPublic.defaultPrinter,
                            onChange: function () {
                                cswPublic.defaultPrinter = cswPrivate.printerSel.val();
                                Csw.cookie.set('defaultPrinter', cswPublic.defaultPrinter);
                            }
                        });

                    });

                }());

                cswPublic.zebraJava = document.jzebra;

                cswPublic.print = function (eplText, pictos) {
                    if (false === Csw.isNullOrEmpty(eplText)) {
                        cswPublic.zebraJava.findPrinter(cswPublic.defaultPrinter);

                        //Case 27885. No Zebra support for images, for now.

                        //var cswEpl = Csw.delimitedString(eplText, { delimiter: '|', spaceToDelimited: false, removeDuplicates: false });

                        //var addPicto = function(eplSubStr) {
                        //    var ret = null;
                        //    if (pictos && pictos.length > 0) {
                        //        pictos.forEach(function (pic) {
                        //            if (eplSubStr.toLowerCase().indexOf(pic.FileName) >= 0) {
                        //                var url = Csw.window.getPath() + pic.FileURL + '&guid=' + window.Ext.id();
                        //                ret = cswPublic.zebraJava.appendImage(url, 'ZPLII');
                        //                cswPublic.zebraJava.append('\n');
                        //            }
                        //        });
                        //    }
                        //    return ret;
                        //};


                        //cswEpl.each(function(eplSubStr) {
                        //    if (eplSubStr.indexOf('CSWPICTO') === 0) {
                        //        var pictoStr = 'F0';
                        //        var eplSubArr = eplSubStr.split(',');
                        //        if (eplSubArr[1]) {
                        //            pictoStr += eplSubArr[1];
                        //        }
                        //        if (eplSubArr[2]) {
                        //            pictoStr += ',' + eplSubArr[2];
                        //        }
                        //        cswPublic.zebraJava.append(pictoStr);
                        //        addPicto(eplSubStr);
                                
                        //    } else {
                        //       cswPublic.zebraJava.append(eplSubStr);
                        //        cswPublic.zebraJava.append('\n');
                        //    }
                        //});

                        cswPublic.zebraJava.append(eplText);

                        var counter = 0;
                        while(!cswPublic.zebraJava.isDoneAppending()) {
                           counter += 1;
                            //sit in timeout    
                            if (counter > 100000) {
                                //I don't trust jZebra. Exit eventually.
                                $.CswDialog('AlertDialog', { text: 'An error occurred waiting for the printer. Please try again.' });
                                break;
                            }
                        }
                        cswPublic.zebraJava.print();
                    } else {
                        $.CswDialog('AlertDialog', { text: 'No EPL text submitted for print.' });
                    }
                };

                cswPublic.findPrinters = function () {
                    //This will populate cswPrivate.printerSel with available printers
                    cswPublic.zebraJs.findPrinters(cswPublic.defaultPrinter);
                };

                cswPublic.setDefaultPrinter = function (defaultPrinter) {
                    cswPublic.defaultPrinter = defaultPrinter;
                    Csw.cookie.set('defaultPrinter', defaultPrinter);
                };

                cswPublic.onZebraReady = function (onSuccess) {
                    cswPublic.zebraJs.monitorApplet('findPrinter', function () {
                        cswPublic.findPrinters();
                        Csw.tryExec(onSuccess);
                    }, 'Init printers list');
                };

                (function _post() {
                    //cswPublic.onZebraReady(cswPrivate.onSuccess);
                }());

            });
            return cswPublic;
        });

}());
