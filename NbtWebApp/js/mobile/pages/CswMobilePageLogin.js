/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobile.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../../main/tools/CswCookie.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../main/tools/CswArray.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

//#region CswMobilePageLogin

function CswMobilePageLogin(loginDef,$page,mobileStorage,loginSuccess) {
    /// <summary>
    ///   Login Page class. Responsible for generating a Mobile login page.
    /// </summary>
    /// <param name="loginDef" type="Object">Login definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="loginSuccess" type="Function">Function to execute on login success.</param>
    /// <returns type="CswMobilePageLogin">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id = CswMobilePage_Type.login.id;
    var title = CswMobilePage_Type.login.title;
    var loginSuffix = '_login';
    var $contentPage = $page.find('#' + id).find('div:jqmData(role="content")');
    var contentDivId = id + loginSuffix;
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);
    
    
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
        
        contentDivId = id + loginSuffix;
        
        if( !isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        
        var buttons = { };
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        
        pageDef = makeMenuButtonDef(p, id, buttons, mobileStorage);

        getContent();
    })(); //ctor
    
    function getContent() {
        $content = ensureContent($content, contentDivId);
        
        $content.append('<p style="text-align: center;">Login to Mobile Inspection Manager</p><br/>');
        var $customerId = $('<input type="text" id="login_customerid" placeholder="Customer Id" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $username = $('<input type="text" id="login_username" placeholder="User Name" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $password = $('<input type="password" id="login_password" placeholder="Password" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $loginBtn = $('<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>')
                            .appendTo($content)
                            .unbind('click')
                            .bind('click', function() {
                                return startLoadingMsg(function() { onLoginSubmit(); });
                            });
        
        if( !isNullOrEmpty($contentPage) && $contentPage.length > 0 ) {
            $contentPage.append($content);
        }
        
        $customerId.clickOnEnter($loginBtn);
        $username.clickOnEnter($loginBtn);
        $password.clickOnEnter($loginBtn);

        function onLoginSubmit() {
            var authenticateUrl = '/NbtWebApp/wsNBT.asmx/Authenticate';
            if (mobileStorage.amOnline()) {
                var userName = $username.val();
                var accessId = $customerId.val();

                var ajaxData = {
                    'AccessId': accessId, //We're displaying "Customer ID" but processing "AccessID"
                    'UserName': userName,
                    'Password': $password.val(),
                    ForMobile: true
                };

                CswAjaxJson({
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
    this.contentDivId = contentDivId;
    this.pageDef = pageDef;
    this.id = id;
    this.title = title;
    this.getContent = getContent;

    //#endregion public, priveleged
}

//#endregion CswMobilePageLogin