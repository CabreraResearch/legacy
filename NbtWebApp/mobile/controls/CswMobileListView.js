/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="ICswMobileWebControls.js" />
/// <reference path="../globals/CswMobileEnums.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

//#region CswMobileListView

CswMobileListView.inheritsFrom(ICswMobileWebControls);

function CswMobileListView(listDef, $parent, bindEvent) {
    "use strict";
    /// <summary>
    ///   Menu button class. Responsible for creating Mobile menu buttons suitable for consumption by a header/footer.
    ///   Menu buttons must be tied to static pages to wire their events properly, with the exception of 'Back'.
    /// </summary>
    /// <param name="buttonDef" type="Object">Button definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="bindEvent" type="CswDomElementEvent">Event type to bind an onEvent function.</param>
    /// <returns type="CswMobileListView">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private 

    ICswMobileWebControls.call(this);
    
    var liSuffix = '_li',
        ulSuffix = '_ul',
        aSuffix = '_a',
        eventName = CswDomElementEvent.click.name,
        o, $control, classes, styles, cssClass, enabled, visible, listId;
    
    if (false === isNullOrEmpty(bindEvent)) {
        eventName = bindEvent.name;
    }
    
    //ctor
    (function() {
            var p = {
                ID: '',
                enabled: true,
                visible: true,
                cssStyles: { 'display': '' },
                cssClass: '',
                'data-filter': false,
                'data-inset': true,
                showLoading: true
                //onClick: '' //function () {}
            }, 
            $ul, ulAttr;
        if (listDef) $.extend(p, listDef);
        
        if (isNullOrEmpty($parent)) {
            throw ('Cannot create a list view without a parent');
        } else {
            listId = p.ID + ulSuffix;
            $ul = $parent.find('#' + listId);
            
            if (isNullOrEmpty($ul) || $ul.length === 0) {
                $ul = $('<ul id="' + listId + '"></ul>').appendTo($parent);
            }

            ulAttr = {
                'data-filter': p['data-filter'],
                'data-role': 'listview',
                'data-inset': p['data-inset']
            };

            $ul.CswAttrNonDom(ulAttr)
                .css(p.cssStyles);

            classes = p.cssClass.split(' ');

            if (classes.length > 0) {
                $ul.addClass(p.cssClass);
            }

            if (p.showLoading) {
                $ul.bind('click', startLoadingMsg);
            }

            o = p;
            cssClass = p.cssClass;
            $control = $ul.trigger('create');
        }
    })(); //ctor
    
    function addListItemLink(id, text, onEvent, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="text" type="String">Text to display</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var p = {
            attr: { 
                'data-icon': false,
                'data-url': id
            },
            icon: ''
        };
        if (options) $.extend(p, options);

        var $li = addListItem(id, '', onEvent, p);
        $li.CswLink('init', { ID: id + aSuffix, href: 'javascript:void(0);', value: text })
                                              .css('white-space', 'normal')
                                              .CswAttrNonDom({
                                              'data-identity': id,
                                              'data-url': id
                                          });
        $control.trigger('create');
        return $li;
    }
    
    function addListItemLinkHtml(id, $html, onEvent, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="$html" type="jQuery">HTML to append</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var p = {
            attr: { 
                'data-icon': false,
                'data-url': id
            },
            icon: ''
        };
        if (options) $.extend(p, options);

        var $li = addListItemLink(id, '', onEvent, p);
        var $a = $li.find('#' + id + aSuffix);
        if (!isNullOrEmpty($a) && $a.length !== 0) {
            $a.append($html);
        }
        $control.trigger('create');
        return $li;
    }
    
    function addListItemHtml(id,$html, onEvent, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="$html" type="jQuery">HTML to append</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var p = {
            attr: { 
                'data-icon': false,
                'data-url': id
            },
            icon: ''
        };
        if(options) $.extend(p, options);

        var $li = addListItem(id, '', onEvent, p);
        $li.append($html);
        $control.trigger('create');
        return $li;
    }
    
    function addListItem(id, text, onEvent, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="text" type="String">Text to display</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var p = {
            attr: { 
                'data-icon': false,
                'data-url': id
            },
            icon: ''
        };
        if(options) $.extend(p, options);

        var $li = $control.find('#' + id + '_li');
        if (!isNullOrEmpty($li) && $li.length > 0) {
            $li.empty();
        } else {
            $li = $('<li id="' + id + liSuffix + '"></li>')
                        .appendTo($control);
        }
        $li.CswAttrNonDom(p.attr);
        if (!isNullOrEmpty(text)) {
            $li.text(text);
        }
        if (!isNullOrEmpty(p.icon)) {
            $li.append('<img src="' + p.icon + '" class="ui-li-thumb"/>');
        }
        
        doEventBind($li, onEvent);
        $control.trigger('create');
        return $li;
    }
    
    function doEventBind($li,onEvent) {
        /// <summary> Binds an event to the list item on either the global eventName or on the { name: eventName, event: function() }</summary>
        /// <param name="$li" type="JQuery">LI Element</param>
        /// <param name="onEvent" type="String">Text to display</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        if (false === isNullOrUndefined(onEvent)) {
            if (isFunction(onEvent)) {
                $li.bind(eventName, onEvent);
            }
            else if (contains(onEvent, 'name') &&
                     contains(onEvent, 'event')) {
                var name = onEvent.name,
                    event = onEvent.event;
                if (false === isNullOrEmpty(name) && isFunction(event)) {
                    $li.bind(name, event);
                }
            }
        }
    }

    //#endregion private 
    
    //#region public, priveleged

    this.$control = $control;
    this.classes = classes;
   
//    if( !isNullOrEmpty(o.onClick) ) {
//        this.unbindEvents(CswDomElementEvent.click);
//        this.setEvent(CswDomElementEvent.click, o.onClick);
//        this.bindEvents(CswDomElementEvent.click);
//    }

    this.addListItemLink = addListItemLink;
    this.addListItemLinkHtml = addListItemLinkHtml;
    this.addListItem = addListItem;
    this.addListItemHtml = addListItemHtml;
    //#endregion public, priveleged
}

//#endregion CswMobileListView