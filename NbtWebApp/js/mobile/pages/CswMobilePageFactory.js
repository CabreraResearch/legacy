/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../CswMobileTools.js" />

//#region CswMobilePageFactory

function CswMobilePageFactory(pageDef, $parent) {
	/// <summary>
	///   Page factory class. Responsible for generating a Mobile page.
	/// </summary>
	/// <param name="pageDef" type="Object">JSON definition of content to display, including header/footerDef properties.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
	/// <returns type="CswMobilePageFactory">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
	var mobileHeader, mobileFooter, $content;
	
	var p = {
		ParentId: undefined,
		level: 1,
		DivId: '',       // required
		HeaderText: '',
		$content: $(''),
		headerDef: { },
		footerDef: { },
	    theme: 'b'
	};

	if (pageDef) {
		$.extend(p, pageDef);
	}

	p.DivId = makeSafeId({ ID: p.DivId });

	var $pageDiv = $('#' + p.DivId);

	var firstInit = (isNullOrEmpty($pageDiv) || $pageDiv.length === 0);
			
	if (firstInit) {
		$pageDiv = $parent.CswDiv('init', { ID: p.DivId })
			.CswAttrXml({
					'data-role': 'page',
					'data-url': p.DivId,
					'data-title': p.HeaderText,
					'data-rel': 'page'
				});
	}
	var headerDef = {
		buttons: { },
		ID: p.DivId,
		text: p.HeaderText,
		dataId: 'csw_header',
		dataTheme: p.theme
	};
	if(p.headerDef) $.extend(headerDef, p.headerDef);
	
	mobileHeader = new CswMobilePageHeader(headerDef, $pageDiv);

    $content = $pageDiv.find('div:jqmData(role="content")');
    
	if( !isNullOrEmpty($content) && $content.length > 0) {
	    $content.empty();
	    $content = $pageDiv.CswDiv('init', { ID: p.DivId + '_content' })
			.CswAttrXml({ 'data-role': 'content', 'data-theme': p.theme })
			.append(p.$content);
	}

	var footerDef = {
		buttons: { },
		ID: p.DivId,
		dataId: 'csw_footer',
		dataTheme: p.theme
	};
	if(p.footerDef) $.extend(footerDef, p.footerDef);
	
	mobileFooter = new CswMobilePageFooter(footerDef, $pageDiv);
	
	//#endregion private	
	
	//#region public, priveleged

	this.mobileHeader = mobileHeader;
	this.mobileFooter = mobileFooter;
	this.$content = $content;
    this.$pageDiv = $pageDiv;
	
	//#region public, priveleged
}

//#endregion CswMobilePageFactory