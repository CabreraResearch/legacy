///// <reference path="~app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswNodePreview';

//    var methods = {
//        'open': function (options) {
//            var o = {
//                ID: '',
//                nodeid: '',
//                cswnbtnodekey: '',
//                eventArg: {},
//                delay: 1500
//            };
//            if (options) Csw.extend(o, options);

//            function _fixDimensions() {
//                // Make sure preview div is within the window
//                var windowX = $(window).width() - 10;
//                var windowY = $(window).height() - 10;
//                var divwidth = $div.width();
//                var divheight = $div.height();
//                var X = o.eventArg.pageX + 20; // move it to the right of the cursor, to keep from preventing click events
//                var Y = o.eventArg.pageY;

//                if (X + divwidth > windowX) X = windowX - divwidth;
//                // this doesn't work with page scrolling
//                // if(Y + divheight > windowY) Y = windowY - divheight;

//                $div.css({
//                    maxWidth: windowX,
//                    maxHeight: windowY,
//                    top: Y + 'px',
//                    left: X + 'px'
//                });
//                $div.css('z-index', '100');
//            } // _fixDimensions()

//            function _loadPreview() {
//                $div.show();
//                var parent = Csw.literals.factory($div);
//                //$div.CswNodeTabs({
//                Csw.layouts.tabsAndProps(parent, {
//                    ID: o.ID + 'tabs',
//                    nodeids: [o.nodeid],
//                    cswnbtnodekeys: [o.cswnbtnodekey],
//                    EditMode: Csw.enums.editMode.Preview,
//                    AjaxWatchGlobal: false,
//                    ShowAsReport: false,
//                    onInitFinish: function (AtLeastOneProp) {
//                        $loadingspan.remove();
//                        if (AtLeastOneProp) {
//                            _fixDimensions();
//                        } else {
//                            $div.hide();
//                        }
//                    }
//                });
//            } // _loadPreview()

//            var $div = $('<div id="' + o.ID + '" class="CswNodePreview"></div>')
//                                .css({
//                                    position: 'absolute',
//                                    overflow: 'auto',
//                                    border: '1px solid #003366',
//                                    padding: '2px',
//                                    backgroundColor: '#ffffff'
//                                })
//                                .appendTo('body')
//                                .hide();

//            _fixDimensions();

//            var $loadingspan = $('<div style="font-style: italic;">&nbsp;&nbsp;&nbsp;Loading...</div>')
//                                        .appendTo($div);

//            var timeoutHandle = setTimeout(_loadPreview, o.delay);
//            $div.data('timeoutHandle', timeoutHandle);

//            return $div;
//        },
//        'close': function () {
//            var $div = $(this);
//            if ($div.length > 0) {
//                clearTimeout($div.data('timeoutHandle'));
//                // Clear all node previews, in case other ones are hanging around
//                $('.CswNodePreview').remove();
//            }
//        }
//    };


//    // Method calling logic
//    $.CswNodePreview = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };

//    $.fn.CswNodePreview = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };

//})(jQuery);

