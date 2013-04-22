/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.text = Csw.properties.register('text',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var cswPrivate = Csw.object();

                cswPrivate.required = nodeProperty.propData.required;
                cswPrivate.value = nodeProperty.propData.values.text;
                cswPrivate.size = nodeProperty.propData.values.size;
                cswPrivate.maxlength = nodeProperty.propData.values.maxlength;
                cswPrivate.regex = nodeProperty.propData.values.regex;
                cswPrivate.regexmsg = nodeProperty.propData.values.regexmsg;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val) {
                        cswPrivate.value = val;
                        updateProp(val);
                    }
                });

                var updateProp = function (val) {
                    nodeProperty.propData.values.text = val;
                    if (text) {
                        text.val(val);
                    }
                    if (span) {
                        span.remove();
                        span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                    }
                };

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.value });
                } else {

                    var regex_name = '';
                    if (false === Csw.isNullOrEmpty(cswPrivate.regex)) {
                        regex_name = 'text_regex_' + nodeProperty.propid;
                    }

                    var text = nodeProperty.propDiv.input({
                        name: nodeProperty.name,
                        type: Csw.enums.inputTypes.text,
                        value: cswPrivate.value,
                        size: cswPrivate.size,
                        cssclass: 'textinput ' + regex_name,
                        onChange: function(val) {
                            cswPrivate.value = val;
                            nodeProperty.propData.values.text = val;
                            nodeProperty.broadcastPropChange(val);
                        },
                        isRequired: nodeProperty.isRequired(),
                        maxlength: cswPrivate.maxlength
                    });

                    text.required(nodeProperty.isRequired());
                }

                if (false === Csw.isNullOrEmpty(cswPrivate.regex)) {
                    var Message = "invalid value";
                    if (false === Csw.isNullOrEmpty(cswPrivate.regexmsg)) {
                        Message = cswPrivate.regexmsg;
                    }

                    $.validator.addMethod(regex_name, function() {

                        var return_val = true;

                        if ((false === cswPrivate.required) && '' !== cswPrivate.value) {
                            var regex_obj = new RegExp(cswPrivate.regex);
                            return_val = regex_obj.test(cswPrivate.value);
                        }

                        return (return_val);

                    }, Message);
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());