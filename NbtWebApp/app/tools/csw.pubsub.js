/// <reference path="~/app/CswApp-vsdoc.js" />

/* globals Csw:false, $:false */
(function () {
    'use strict';

    function makePubSubMethods(privateBindingObj, nameSpace) {
        privateBindingObj = privateBindingObj || $({});



        nameSpace.register('subscribe', function () {
            /// <summary>
            ///     Attach an event handler function for one or more events to the selected elements.
            ///     &#10;1 - on(events, selector, data, handler)
            ///     &#10;2 - on(events-map, selector, data)
            /// </summary>
            /// <param name="types" type="String">
            ///     One or more space-separated event types and optional namespaces, such as "click" or "keydown.myPlugin".
            /// </param>
            /// <param name="selector" type="String">
            ///     A selector string to filter the descendants of the selected elements that trigger the event. If the selector is null or omitted, the event is always triggered when it reaches the selected element.
            /// </param>
            /// <param name="data" type="Anything">
            ///     Data to be passed to the handler in event.data when an event is triggered.
            /// </param>
            /// <param name="fn" type="Function">
            ///     A function to execute when the event is triggered. The value false is also allowed as a shorthand for a function that simply does return false.
            /// </param>
            /// <returns type="jQuery" />
            privateBindingObj.on.apply(privateBindingObj, arguments);
        });

        nameSpace.register('unsubscribe', function () {
            /// <summary>
            ///     Remove an event handler.
            ///     &#10;1 - off(events, selector, handler)
            ///     &#10;2 - off(events-map, selector)
            /// </summary>
            /// <param name="types" type="String">
            ///     One or more space-separated event types and optional namespaces, or just namespaces, such as "click", "keydown.myPlugin", or ".myPlugin".
            /// </param>
            /// <param name="selector" type="String">
            ///     A selector which should match the one originally passed to .on() when attaching event handlers.
            /// </param>
            /// <param name="fn" type="Function">
            ///     A handler function previously attached for the event(s), or the special value false.
            /// </param>
            /// <returns type="jQuery" />
            privateBindingObj.off.apply(privateBindingObj, arguments);
        });


        nameSpace.register('publish', function () {
            /// <summary>
            ///     Execute all handlers and behaviors attached to the matched elements for the given event type.
            ///     &#10;1 - trigger(eventType, extraParameters)
            ///     &#10;2 - trigger(event)
            /// </summary>
            /// <param name="type" type="String">
            ///     A string containing a JavaScript event type, such as click or submit.
            /// </param>
            /// <param name="data" type="Object">
            ///     Additional parameters to pass along to the event handler.
            /// </param>
            /// <returns type="jQuery" />
            privateBindingObj.trigger.apply(privateBindingObj, arguments);
        });


        nameSpace.register('pubsSubs', function () {
            var ret = {};
            if (Csw.clientSession.isDebug()) {
                Csw.iterate(privateBindingObj[0], function (prop, key) {
                    if (prop.events) {
                        ret = prop.events;
                        return false;
                    }
                });
            }
            return ret;
        });
    }

    makePubSubMethods($({}), Csw);
    makePubSubMethods($({}), Csw.properties);


    Csw.register('scopeNewPubSubBindings', makePubSubMethods);

}());