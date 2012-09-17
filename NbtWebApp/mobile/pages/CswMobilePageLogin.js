/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
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
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

//#region CswMobilePageLogin

function CswMobilePageLogin(loginDef, $parent, mobileStorage, loginSuccess, $contentRole) {
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
    var id, title, contentDivId, $content,
        divSuffix = '_login';
    
    //ctor
    (function () {
        pageDef = {
            level: -1,
            DivId: '',
            buttons: [CswMobileFooterButtons.fullsite, CswMobileFooterButtons.help],
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (loginDef) {
            $.extend(pageDef, loginDef);
        }

        id = tryParseString(pageDef.DivId, CswMobilePage_Type.login.id);
        contentDivId = id + divSuffix;
        title = tryParseString(pageDef.title, CswMobilePage_Type.login.title);
        $content = ensureContent($contentRole, contentDivId);

        getContent();
    })();  //ctor
    
    function getContent() {
        $content = ensureContent($contentRole, contentDivId);
        
        $content.append('<p style="text-align: center;">Login to Mobile Inspection Manager</p><br/>');
        var loginFailure = mobileStorage.getItem('loginFailure');
        if (loginFailure)
        {
            $content.append('<span class="error">' + loginFailure + '</span><br/>');
        }
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
        
        if (false === isNullOrEmpty($contentRole) && $contentRole.length > 0 ) {
            $contentRole.append($content);
        }
        
        $customerId.clickOnEnter($loginBtn);
        $username.clickOnEnter($loginBtn);
        $password.clickOnEnter($loginBtn);

        function onLoginSubmit() {
            var authenticateUrl = 'wsNBT.asmx/authenticate';
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
                            doSuccess(loginSuccess, data, userName, accessId);
                        },
                        error: function() {
                            onError();
                        }
                    });
            }

            } //onLoginSubmit()
        $contentRole.append($content);
        return $content;
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $pageDiv: $parent,
        $contentRole: $contentRole,
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageLogin