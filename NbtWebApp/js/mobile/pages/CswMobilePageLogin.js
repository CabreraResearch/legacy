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

    var $content = '';
    var pageDef = { };
    var id = 'logindiv';
    var title = 'ChemSW Live';
    
    //ctor
    (function() {
        
        var p = {
            level: -1,
            DivId: '',
            title: '',
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
            theme: CswMobileGlobal_Config.theme,
            onHelpClick: null // function () {}
        };
        if (loginDef) $.extend(p, loginDef);
        
        if(!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        
        if( isNullOrEmpty(p.footerDef) ) {
            p.footerDef = { buttons: { } };
        }
        if (isNullOrEmpty(p.footerDef.buttons)) {
            p.footerDef.buttons.fullsite = makeFooterButtonDef(CswMobileFooterButtons.fullsite, id);
            p.footerDef.buttons.help = makeFooterButtonDef(CswMobileFooterButtons.help, id, p.onHelpClick);
        }
       
        pageDef = p;
        
        getContent();
    })(); //ctor
    
    function getContent() {
        var $contentPage = $parent.find('#' + id).find('div:jqmData(role="content")');
        $contentPage.empty();
        
        var loginContent = '<p style="text-align: center;">Login to Mobile Inspection Manager</p>';
        loginContent += '<input type="textbox" id="login_customerid" placeholder="Customer Id" /><br>';
        loginContent += '<input type="textbox" id="login_username" placeholder="User Name" /><br>';
        loginContent += '<input type="password" id="login_password" placeholder="Password" /><br>';
        loginContent += '<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>';
        
        $content = $(loginContent);
        if( !isNullOrEmpty($contentPage) && $contentPage.length > 0 ) {
            $contentPage.append($content);
        }
        
        var $loginBtn = $content.find('#loginsubmit')
                                .unbind('click')
                                .bind('click', function() {
                                    return startLoadingMsg(function() { onLoginSubmit(); });
                                });
        
        $content.find('#login_customerid').clickOnEnter($loginBtn);
        $content.find('#login_username').clickOnEnter($loginBtn);
        $content.find('#login_password').clickOnEnter($loginBtn);

        function onLoginSubmit() {
            var authenticateUrl = '/NbtWebApp/wsNBT.asmx/Authenticate';
            
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
                            if (!isNullOrEmpty(loginSuccess)) {
                                loginSuccess(data, userName, accessId);
                            }
                        },
                        error: function() {
                            onError();
                        }
                    });
            }

        } //onLoginSubmit() 
        return $content;
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;

    //#endregion public, priveleged
}

//#endregion CswMobilePageLogin