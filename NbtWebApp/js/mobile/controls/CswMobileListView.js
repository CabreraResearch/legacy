/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="ICswMobileWebControls.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileListView

CswMobileListView.inheritsFrom(ICswMobileWebControls);

function CswMobileListView(listDef, $parent) {
    /// <summary>
    ///   Menu button class. Responsible for creating Mobile menu buttons suitable for consumption by a header/footer.
    ///   Menu buttons must be tied to static pages to wire their events properly, with the exception of 'Back'.
    /// </summary>
    /// <param name="buttonDef" type="Object">Button definitional data.</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobileListView">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //#region private 

    ICswMobileWebControls.call(this);
    
    var _o, _$control, _classes, _styles, _cssClass, _enabled, _visible;
    var liSuffix = '_li';
    var ulSuffix = '_ul';
    var aSuffix = '_a';
    
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
            'showLoading': true,
            onClick: '' //function () {}
        };

        if (listDef) $.extend(p, listDef);

        if (isNullOrEmpty($parent)) {
            throw ('Cannot create a list view without a parent');
        }

        var $ul = $parent.find('#' + p.ID + ulSuffix);
        if (isNullOrEmpty($ul) || $ul.length === 0) {
            $ul = $('<ul id="' + p.ID + ulSuffix + '"></ul>').appendTo($parent);
        }

        var ulAttr = {
            'data-filter': p['data-filter'],
            'data-role': 'listview',
            'data-inset': p['data-inset']
        };

        $ul.CswAttrXml(ulAttr)
            .css(p.cssStyles);

        var classes = p.cssClass.split(' ');

        if (classes.length > 0) {
            $ul.addClass(p.cssClass);
        }

        if (p.showLoading) {
            $ul.bind('click', function() { $.mobile.showPageLoadingMsg(); });
        }

        _o = p;
        _classes = classes;
        _cssClass = p.cssClass;
        _$control = $ul;
    })();
    
    function addListItemLink(id,text,onClick,options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="text" type="String">Text to display</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var o = {
            'data-icon': false,
            'data-url': id
        };
        if(options) $.extend(o, options);

        var $li = addListItem(id, '', onClick, o);
        $li.CswLink('init', { ID: id + aSuffix, href: 'javascript:void(0);', value: text })
											  .css('white-space', 'normal')
											  .CswAttrXml({
											  'data-identity': id,
											  'data-url': id
										  });
        return $li;
    }
    
    function addListItemLinkHtml(id,$html,onClick,options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="$html" type="jQuery">HTML to append</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var o = {
            'data-icon': false,
            'data-url': id
        };
        if (options) $.extend(o, options);

        var $li = addListItemLink(id, '', onClick, o);
        var $a = $li.find('#' + id + aSuffix);
        if (!isNullOrEmpty($a) && $a.length !== 0) {
            $a.append($html);
        }

        return $li;
    }
    
    function addListItemHtml(id,$html,onClick, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="$html" type="jQuery">HTML to append</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var o = {
            'data-icon': false,
            'data-url': id
        };
        if(options) $.extend(o, options);

        var $li = addListItem(id, '', onClick, o);
        $li.append($html);
        return $li;
    }
    
    function addListItem(id, text, onClick, options) {
        /// <summary>
        ///   Add a list item to the UL
        /// </summary>
        /// <param name="id" type="String">Element Id</param>
        /// <param name="text" type="String">Text to display</param>
        /// <param name="options" type="Object">JSON options to append.</param>
        /// <returns type="jQuery">The list item created.</returns>
        var o = {
            'data-icon': false,
            'data-url': id
        };
        if(options) $.extend(o, options);

        var $li = _$control.find('#' + id + '_li');
        if( !isNullOrEmpty($li) && $li.length > 0) {
            $li.empty();
        } else {
            $li = $('<li id="' + id + liSuffix + '"></li>')
                        .appendTo(_$control);
        }
        $li.CswAttrXml(o);
        if(!isNullOrEmpty(text)) {
            $li.text(text);
        }
        if(onClick) {
            $li.bind('click', onClick);
        }
        
        return $li;
    }
    
    //#endregion private 
    
    //#region public, priveleged

    this.$control = _$control;
    this.classes = _classes;
   
    if( !isNullOrEmpty(_o.onClick) ) {
        this.unbindEvents(CswDomElementEvent.click);
        this.setEvent(CswDomElementEvent.click, _o.onClick);
        this.bindEvents(CswDomElementEvent.click);
    }

    this.addListItemLink = addListItemLink;
    this.addListItemLinkHtml = addListItemLinkHtml;
    this.addListItem = addListItem;
    //#endregion public, priveleged
}

//#endregion CswMobileListView