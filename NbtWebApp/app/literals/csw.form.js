/// <reference path="~app/CswApp-vsdoc.js" />


(function _cswform() {
    'use strict';
    Csw.literals.form = Csw.literals.form ||
        Csw.literals.register('form', function (options) {
            /// <summary> Create or extend an HTML <form /> and return a Csw.form object
            ///     &#10;1 - form(options)
            ///     &#10;2 - form($jqueryElement)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the form.</para>
            /// </param>
            /// <returns type="form">A form object</returns>
            var cswPrivate = {
                $parent: '',
                ID: ''
            };
            var cswPublic = {};

            (function _pre() {
                var html = '',
                    attr = Csw.makeAttr();
                var $form;

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                html += '<form ';
                attr.add('id', cswPrivate.ID);
                html += attr.get();
                html += '>';
                html += '</form>';
                $form = $(html);

                Csw.literals.factory($form, cswPublic);

                cswPrivate.$parent.append(cswPublic.$);

            } ());

            cswPublic.validator = {};
            cswPublic.initValidator = function () {
                // Validation
                cswPublic.validator = cswPublic.$.validate({
                    highlight: function (element) {
                        var $elm = $(element);
                        $elm.attr('csw_invalid', '1');
                        $elm.animate({ backgroundColor: '#ff6666' });
                    },
                    unhighlight: function (element) {
                        var $elm = $(element);
                        if ($elm.attr('csw_invalid') === '1')  // only unhighlight where we highlighted
                        {
                            $elm.css('background-color', '#66ff66');
                            $elm.attr('csw_invalid', '0');
                            setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
                        }
                    }
                });
            }; // initValidator()

            cswPublic.isFormValid = function () {
                return cswPublic.$.valid();
            };

            (function _post() {
                cswPublic.initValidator();
            })();

            return cswPublic;
        });



} ());

