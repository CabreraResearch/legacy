/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobile.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="../../jquery/common/CswCookie.js" />
/// <reference path="CswMobilePageFactory.js" />

//#region CswMobilePageLogin

function CswMobilePageLogin(loginDef,$parent,mobileStorage,loginSuccess) {
	/// <summary>
	///   Login Page class. Responsible for generating a Mobile login page.
	/// </summary>
    /// <param name="loginDef" type="Object">Login definitional data.</param>
	/// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="loginSuccess" type="Function">Function to execute on login success.</param>
	/// <returns type="CswMobilePageLogin">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var loginContent = '<p style="text-align: center;">Login to Mobile Inspection Manager</p>';
	loginContent += '<input type="textbox" id="login_customerid" placeholder="Customer Id" /><br>';
	loginContent += '<input type="textbox" id="login_username" placeholder="User Name" /><br>';
	loginContent += '<input type="password" id="login_password" placeholder="Password" /><br>';
	loginContent += '<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>';
    
    var authenticateUrl = '/NbtWebApp/wsNBT.asmx/Authenticate';
    
    var p = {
		level: -1,
		DivId: 'logindiv',       // required
		HeaderText: 'ChemSW Live',
        headerDef: { },
        theme: 'b',
        $content: $(loginContent),
        onHelpClick: function () {}
    };
    if(loginDef) $.extend(p, loginDef);

    var pageDef = p;
    delete pageDef.onHelpClick;
    
    if( isNullOrEmpty(pageDef.footerDef)) {
        pageDef.footerDef = { };
        pageDef.footerDef.buttons = { };
        pageDef.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, p.DivId);
        pageDef.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, p.DivId, p.onHelpClick);
    }
    
    var loginDiv = new CswMobilePageFactory(pageDef, $parent);
    var loginHeader = loginDiv.mobileHeader;
    var loginFooter = loginDiv.mobileFooter;
    var $content = loginDiv.$content;
    			
	var loginFailure = mobileStorage.getItem('loginFailure');
	if( !isNullOrEmpty(loginFailure) ) {
		loginHeader.addToHeader(loginFailure)
			       .css('color','yellow');
	}
			
	$('#loginsubmit')
		.unbind('click')
		.bind('click', function() {
			return startLoadingMsg(function() { onLoginSubmit(); });
	});
	$('#login_customerid').clickOnEnter($('#loginsubmit'));
	$('#login_username').clickOnEnter($('#loginsubmit'));
	$('#login_password').clickOnEnter($('#loginsubmit'));
	
	function onLoginSubmit() {
		if (mobileStorage.amOnline()) {
			var userName = $('#login_username').val();
			var accessId = $('#login_customerid').val();

			var ajaxData = {
				'AccessId': accessId, //We're displaying "Customer ID" but processing "AccessID"
				'UserName': userName,
				'Password': $('#login_password').val(),
				ForMobile: true
			};

			CswAjaxJSON({
					formobile: true,
					//async: false,
					url: authenticateUrl,
					data: ajaxData,
					onloginfail: function(text) {
						onLoginFail(text, mobileStorage);
					},
					success: function(data) {
					    if( !isNullOrEmpty(loginSuccess) ) {
					        loginSuccess(data, userName, accessId);
					    }
					},
					error: function() {
						onError();
					}
				});
		}

	} //onLoginSubmit() 
	
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.mobileHeader = loginHeader;
    this.mobileFooter = loginFooter;
    this.$pageDiv = loginDiv.$pageDiv;
    //#endregion public, priveleged
}

//#endregion CswMobilePageLogin