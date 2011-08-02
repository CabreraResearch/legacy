/// <reference path="../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../thirdparty/jquery/core/jquery.mobile/jquery.mobile-1.0b1.js" />
/// <reference path="../_Global.js" />
/// <reference path="../mobile/clientdb/CswMobileClientDb.js" />
/// <reference path="../mobile/clientdb/CswMobileClientDbResources.js" />
/// <reference path="../CswClientDb.js" />
/// <reference path="../CswEnums.js" />
/// <reference path="../CswProfileMethod.js" />
/// <reference path="../mobile/CswMobileTools.js" />
/// <reference path="../mobile/sync/CswMobileBackgroundTask.js" />
/// <reference path="../mobile/sync/CswMobileSync.js" />
/// <reference path="controls/CswMobileMenuButton.js" />
/// <reference path="controls/ICswMobileWebControls.js" />
/// <reference path="controls/CswMobilePageFooter.js" />
/// <reference path="controls/CswMobilePageHeader.js" />
/// <reference path="pages/CswMobilePageFactory.js" />
/// <reference path="pages/CswMobilePageViews.js" />
/// <reference path="pages/CswMobilePageHelp.js" />
/// <reference path="pages/CswMobilePageLogin.js" />
/// <reference path="pages/CswMobilePageOnline.js" />
/// <reference path="pages/CswMobilePageSearch.js" />
/// <reference path="pages/CswMobilePageOffline.js" />


//var profiler = $createProfiler();

CswAppMode.mode = 'mobile';

;(function($) {
	/// <param name="$" type="jQuery" />

	$.fn.CswMobile = function(options) {
		/// <summary>
		///   Generates the Nbt Mobile page
		/// </summary>
		var $body = this;

		//#region Resource Initialization
		
		var opts = {
			ViewsListUrl: '/NbtWebApp/wsNBT.asmx/GetViewsList',
			ViewUrl: '/NbtWebApp/wsNBT.asmx/GetView',
			ConnectTestUrl: '/NbtWebApp/wsNBT.asmx/ConnectTest',
			ConnectTestRandomFailUrl: '/NbtWebApp/wsNBT.asmx/ConnectTestRandomFail',
			//UpdateViewUrl: '/NbtWebApp/wsNBT.asmx/UpdateProperties',
			MainPageUrl: '/NbtWebApp/Mobile.html',
			Theme: 'b',
			PollingInterval: 30000, //30 seconds
			RandomConnectionFailure: false
		};

		if (options) {
			$.extend(opts, options);
		}
		
		var mobileStorage = new CswMobileClientDbResources(); 
		
		debugOn(debug);
		
		var ForMobile = true;
		
		var SessionId = mobileStorage.sessionid();
		if(isNullOrEmpty(SessionId)) {
			Logout(mobileStorage);
		}

		var storedViews = mobileStorage.getItem('storedViews');
		
		var mobileSyncOptions = {
			onSync: processModifiedNodes,
			onSuccess: processUpdatedNodes,
			onComplete: function () {
				updatedUnsyncedChanges();
			},
			ForMobile: true
		};

		var mobileSync = new CswMobileSync(mobileSyncOptions, mobileStorage);

		var mobileBackgroundTaskOptions = {
			onSuccess: function () {
				setOnline(mobileStorage);
			},
			onError: function () {
				setOffline();
			},
			onLoginFailure: onLoginFail,
			PollingInterval: opts.PollingInterval,
			taskUrl: opts.ConnectTestUrl,
			ForMobile: ForMobile
		};

		var mobileBgTask = new CswMobileBackgroundTask(mobileStorage, mobileSync, mobileBackgroundTaskOptions);
		
		//#endregion Resource Initialization
		
		var $logindiv = _loadLoginDiv();
		var $viewsdiv = reloadViews();
		var $syncstatus = _makeSyncStatusDiv();
		var $helpdiv = _makeHelpDiv();
		var $sorrycharliediv = _loadSorryCharlieDiv();

		// case 20355 - error on browser refresh
		if (!isNullOrEmpty(SessionId)) {
			$viewsdiv.CswSetPath();
			mobileStorage.setItem('refreshPage', 'viewsdiv');
		} else {
			$logindiv.CswSetPath();
			mobileStorage.setItem('refreshPage', 'logindiv' );
		}
			  
		window.onload = function() {
			if (!isNullOrEmpty(SessionId)) {
				$viewsdiv.CswSetPath();
				$viewsdiv = reloadViews();
			}
			else {
				$logindiv.CswSetPath();
				mobileBgTask.start(
					function() {
						// online
						if( !$logindiv || $logindiv.length === 0 ) {
							$logindiv = _loadLoginDiv();
						}
						$logindiv.CswChangePage();
					},
					function() {
						// offline
						if( !$sorrycharliediv || $sorrycharliediv.length === 0 ) {
							$sorrycharliediv = _loadSorryCharlieDiv();
						}
						$sorrycharliediv.CswChangePage();
					}
				);
			}
		};
		
		function _loadLoginDiv() {

		    var loginDef = { theme: opts.Theme };
		    var loginDiv = new CswMobilePageLogin(loginDef, $body, mobileStorage, function (data,userName,accessId) {
		            SessionId = $.CswCookie('get', CswCookieName.SessionId);
					mobileStorage.sessionid(SessionId);
					mobileStorage.username(userName); 
					mobileStorage.customerid(accessId);
					$viewsdiv = reloadViews();
					$viewsdiv.CswChangePage();  
		        });
		    var $retDiv = loginDiv.$pageDiv;

			return $retDiv;
		}

		function reloadViews() {
			/// <summary>
			///   Refreshes the viewsdiv
			/// </summary>
			var params = {
				parentlevel: -1,
				level: 0,
				DivId: 'viewsdiv',
				HeaderText: 'Views',
				HideRefreshButton: true,
				HideSearchButton: true,
				HideBackButton: true
			};
			if (!$viewsdiv) {
				$viewsdiv = _addPageDivToBody(params);
			}
			params.onPageShow = function() { return _loadDivContents(params); };

			$viewsdiv.CswUnbindJqmEvents();
			$viewsdiv.CswBindJqmEvents(params);
			return $viewsdiv;
		}
		
		function _loadSorryCharlieDiv(params) {
			var offlineDef = {
				theme: opts.Theme
			};
		    var offlinePage = new CswMobilePageOffline(offlineDef, $body, mobileStorage);
		    var $retDiv = offlinePage.$pageDiv;
			return $retDiv;
		}

	    // ------------------------------------------------------------------------------------
		// List items fetching
		// ------------------------------------------------------------------------------------

		function _loadDivContents(params) {
			var logger = new CswProfileMethod('loadDivContents');
			
			if($('#logindiv')) $('#logindiv').remove();
			
			startLoadingMsg();
			mobileBgTask.reset();
			
			var p = {
				ParentId: '',
				level: 1,
				DivId: '',
				HeaderText: '',
				HideRefreshButton: false,
				HideSearchButton: false,
				json: '',
				SessionId: SessionId,
				PageType: 'search'
			};
			if (params) $.extend(p, params);

			var viewId = (p.level < 2) ? mobileStorage.currentViewId(p.DivId) : mobileStorage.currentViewId();
	   
			var $retDiv = $('#' + p.DivId);

			if (isNullOrEmpty($retDiv) || $retDiv.length === 0 || $retDiv.find('div:jqmData(role="content")').length === 1) {
				if (p.level === 0) {
					p.PageType = 'view';
					if (!mobileStorage.amOnline()) {
						p.json = mobileStorage.fetchCachedViewJson(p.DivId);
						$retDiv = _loadDivContentsJson(p);
					} else {
						p.url = opts.ViewsListUrl;
						$retDiv = _getDivJson(p);
					}
				} else if (p.level === 1) {
					// case 20354 - try cached first
					var cachedJson = mobileStorage.fetchCachedViewJson(viewId);
					p.PageType = 'node';
					if (!isNullOrEmpty(cachedJson)) {
						p.json = cachedJson;
						$retDiv = _loadDivContentsJson(p);
					} else if (mobileStorage.amOnline()) {
						p.url = opts.ViewUrl;
						$retDiv = _getDivJson(p);
					} else {
						stopLoadingMsg();
					}
					
				} else { // Level 2 and up
					var cachedJson = mobileStorage.fetchCachedNodeJson(p.DivId);
					p.PageType = 'tab';
					if( !isNullOrEmpty(cachedJson) ) {
						p.json = cachedJson['subitems'];

						if (!isNullOrEmpty(p.json)) {
							$retDiv = _loadDivContentsJson(p);
						}
					} else {
						stopLoadingMsg();
					}
				}
			}
			cacheLogInfo(logger);
			return $retDiv;
		} // _loadDivContents()

		function _loadDivContentsJson(params) {
			params.parentlevel = params.level;
			var $retDiv = _processViewJson(params);
			return $retDiv;
		}

		function _getDivJson(params) {
			var logger = new CswProfileMethod('getDivJson');
			var $retDiv = undefined;

			var p = {
				url: opts.ViewUrl
			};
			$.extend(p, params);

			var jsonData = {
				SessionId: p.SessionId,
				ParentId: p.DivId,
				ForMobile: ForMobile
			};
		
			CswAjaxJSON({
					//async: false,   // required so that the link will wait for the content before navigating
					formobile: ForMobile,
					url: p.url,
					data: jsonData,
					onloginfail: function(text) { onLoginFail(text, mobileStorage); },
					success: function(data) {
						setOnline(mobileStorage);
						logger.setAjaxSuccess();
						
						p.json = data;
						var searchJson = { };
						if( params.level === 1) {
							searchJson = data['searches'];
						}
						if( params.level !== 0) {
							p.json = data['nodes'];    
						}
						if( params.level < 2) {
							mobileStorage.storeViewJson(p.DivId, p.HeaderText, p.json, params.level, searchJson);
						}
						$retDiv = _loadDivContentsJson(p);
					},
					error: function() {
						onError();
					}
				});
			cacheLogInfo(logger);
			return $retDiv;
		}

		function _processViewJson(params) {
			var logger = new CswProfileMethod('processViewJson');
			var p = {
				ParentId: '',
				DivId: '',
				HeaderText: '',
				json: '',
				parentlevel: '',
				PageType: '',
				level: '',
				HideSearchButton: false,
				HideOnlineButton: false,
				HideRefreshButton: false,
				HideHelpButton: false,
				HideBackButton: false,
				nextTab: ''
			};
			if (params) $.extend(p, params);

			var $retDiv = _addPageDivToBody({
					ParentId: p.ParentId,
					level: p.parentlevel,
					DivId: p.DivId,
					HeaderText: p.HeaderText,
					HideSearchButton: p.HideSearchButton,
					HideOnlineButton: p.HideOnlineButton,
					HideRefreshButton: p.HideRefreshButton,
					HideHelpButton: p.HideHelpButton,
					HideBackButton: p.HideBackButton
				});

			var $content = $retDiv.find('div:jqmData(role="content")').empty();

			var showLoading = (p.PageType !== 'prop');
			var $list = $content.cswUL({cssclass: 'csw_listview', showLoading: showLoading});
			
			for(var key in p.json)
			{
				var item = { };
				$.extend(item, p);
				item.json = { id: key, value: p.json[key] };
				_makeListItemFromJson($list, item)
					.CswAttrXml('data-icon', false) //hides the arrow
					.appendTo($list);
			}

			if( !isNullOrEmpty(p.nextTab) ) {
				var item = { };
				$.extend(item, p);
				item.json = { id: p.nextTab };
				item.PageType = 'tab';
				item.DivId = item.ParentId;
				item.suppressProps = true;
				_makeListItemFromJson($list, item)
					//.CswAttrXml('data-icon', true) //show the arrow
					.appendTo($list);
			}
			
			logger.setAjaxSuccess();
			
			_resetPendingChanges();
			
			if(!mobileStorage.stayOffline()) {
				_toggleOffline(false);
			}
			cacheLogInfo(logger);

			stopLoadingMsg();
			
			return $retDiv;
		} // _processViewJson()

		function _makeListItemFromJson($list, params) {
			var p = {
				ParentId: '',
				DivId: '',
				HeaderText: '',
				json: '',
				PageType: '',
				parentlevel: '',
				level: '',
				HideRefreshButton: false,
				HideSearchButton: false,
				nextTab: '',
				suppressProps: false
			};
			if (params) $.extend(p, params);

			var id = makeSafeId({ ID: p.json['id'] });
			var text = '';

			var IsDiv = (!isNullOrEmpty(id));

			var $retLI = $('');

			switch (p.PageType) {
				case 'search':
						// ignore this
					break;
				case 'node':
					text = p.json['value']['node_name'];
					$retLI = _makeObjectClassContent(p)
											.appendTo($list);
					break;
				case 'tab':
					{
						text = id;
						id = makeSafeId({prefix: id.replace(' ', ''), ID: p.DivId }); //we prefer nodeid_ to be at the end
						$retLI = $('<li></li>')
									.appendTo($list);
						$retLI.CswLink('init', { href: 'javascript:void(0);', value: text })
											.css('white-space', 'normal')
											.CswAttrXml({
											'data-identity': id,
											'data-url': id
										});
						
						var nextTab = (!isNullOrEmpty(p.json['value'])) ? p.json['value']['nexttab'] : '';
						if( !isNullOrEmpty(nextTab)) {
							//we're creating a tab link on a prop
							delete p.json['value']['nexttab'];
						}

						if( !p.suppressProps) {
							//we're creating a tab link which needs child props
							setTimeout(function() {
								_processViewJson({
										ParentId: p.DivId,
										DivId: id,
										HeaderText: text,
										json: p.json['value'],
										parentlevel: p.level,
										level: p.level + 1,
										PageType: 'prop',
										nextTab: nextTab,
										suppressProps: true
									});
							}, 500);
						}
						break;   
					} // case 'prop':
				case 'prop':
					{
						if( !isNullOrEmpty(p.json['value']) && !isNullOrEmpty(p.json['id']) ) {
							_FieldTypeJsonToHtml(p.json['value'], p.DivId, p.json['id'])
								.appendTo($list);
						}
						break;
					}
				default:
					{
						text = p.json['value'];
						$retLI = $('<li></li>').appendTo($list);
						if (IsDiv) {
							$retLI.CswLink('init', { href: 'javascript:void(0);', value: text })
											  .css('white-space', 'normal')
											  .CswAttrXml({
											  'data-identity': id,
											  'data-url': id
										  });
						} else {
							$retLI.val(text);
						}
						break;
					}// default:
			}

			$retLI.unbind('click');
			$retLI.bind('click', function() {
				return startLoadingMsg(function() {
					var par = {ParentId: p.DivId,
						parentlevel: p.parentlevel,
						level: p.parentlevel + 1,
						DivId: id,
						persistBindEvent: true,
						HeaderText: text  };
					var $div = _addPageDivToBody(par);
					par.onPageShow = function() { return _loadDivContents(par); };
					$div.CswUnbindJqmEvents();
					$div.CswBindJqmEvents(par);
					$div.CswChangePage({ reloadPage: true });
				});
			});
			
			return $retLI;
		}// _makeListItemFromJson()

		function _makeObjectClassContent(params) {
			var p = {
				ParentId: '',
				DivId: '',
				HeaderText: '',
				json: '',
				parentlevel: '',
				HideRefreshButton: false,
				HideSearchButton: false
			};
			if (params) $.extend(p, params);

			var $retHtml;
			var Html = '';
			var id = makeSafeId({ ID: p.json['id'] });
			var nodeSpecies = p.json['value']['nodespecies'];
			var NodeName = p.json['value']['node_name'];
			var icon = '';
			if (!isNullOrEmpty(p.json['value']['iconfilename'])) {
				icon = 'images/icons/' + p.json['value']['iconfilename'];
			}
			var ObjectClass = p.json['value']['objectclass'];

			if( nodeSpecies !== 'More' )
			{
				switch (ObjectClass) {
				case "InspectionDesignClass":
					var DueDate = tryParseString(p.json['value']['duedate'],'' );
					var Location = tryParseString(p.json['value']['location'],'' );
					var MountPoint = tryParseString(p.json['value']['target'],'' );
					var Status = tryParseString(p.json['value']['status'],'' );
//Case 22579: just remove for now
//                var UnansweredCnt = 0;

//                p.$xmlitem.find('prop[fieldtype="Question"]').each(function() {
//                    var $question = $(this).clone();
//                    if (isNullOrEmpty($question.children('Answer').text())) {
//                        UnansweredCnt++;
//                    }
//                });

					Html += '<li>';
					Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">';
					if (!isNullOrEmpty(icon))
						Html += '<img src="' + icon + '" class="ui-li-icon"/>';
					Html += '<h2>' + NodeName + '</h2>';
					Html += '<p>' + Location + '</p>';
					Html += '<p>' + MountPoint + '</p>';
					Html += '<p>';
					if (!isNullOrEmpty(Status)) Html += Status + ', ';
					Html += 'Due: ' + DueDate + '</p>';
//                Html += '<span id="' + makeSafeId({ prefix: id, ID: 'unansweredcnt' }) + '" class="ui-li-count">' + UnansweredCnt + '</span>';
					Html += '</a>';
					Html += '</li>';
					break;
				default:
					Html += '<li>';
					if (!isNullOrEmpty(icon))
						Html += '<img src="' + icon + '" class="ui-li-icon"/>';
					Html += '<a data-identity="' + id + '" data-url="' + id + '" href="javascript:void(0);">' + NodeName + '</a>';
					Html += '</li>';
					break;
				}
			} //if( nodeSpecies !== 'More' )
			else {
				Html += '<li>';
				Html += '<h2 id="' + id + '">' + NodeName + '</h2>';
				Html += '</li>';
			}
			$retHtml = $(Html);
			
			return $retHtml;
		}

		function _FieldTypeJsonToHtml(json, ParentId, IdStr) {
			/// <summary>
			///   Converts JSON into DOM content
			/// </summary>
			/// <param name="json" type="Object">A JSON Object</param>
			/// <param name="ParentId" type="String">The ElementID of the parent control (should be a prop)</param>
			/// <param name="IdStr" type="String">The ElementID of the child control</param>
			var $retLi = $('');
			if( !isNullOrEmpty(json)) {
				var FieldType = json['fieldtype'];
				var PropName = json['prop_name'];
				var ReadOnly = isTrue(json['isreadonly']);

				// Subfield values
				var sf_text = tryParseString(json['text'], '');
				var sf_value = tryParseString(json['value'], '');
				var sf_href = tryParseString(json['href'], '');
				var sf_checked = tryParseString(json['checked'], '');
				//var sf_relationship = tryParseString(json['value']['name'], '');
				var sf_required = tryParseString(json['required'], '');
				var sf_units = tryParseString(json['units'], '');
				var sf_answer = tryParseString(json['answer'], '');
				var sf_allowedanswers = tryParseString(json['allowedanswers'], '');
				var sf_correctiveaction = tryParseString(json['correctiveaction'], '');
				var sf_comments = tryParseString(json['comments'], '');
				var sf_compliantanswers = tryParseString(json['compliantanswers'], '');
				var sf_options = tryParseString(json['options'], '');

				$retLi = $('<li id="' + IdStr + '_li"></li>')
					.CswAttrXml('data-icon', false);
				var $label = $('<h2 id="' + IdStr + '_label" style="white-space:normal;" class="csw_prop_label">' + PropName + '</h2>')
					.appendTo(($retLi));

				var $fieldcontain = $('<div class="csw_fieldset" ></div>')
					.appendTo($retLi);

				var $propDiv = $('<div></div>');

				if (FieldType === "Question" &&
					!(sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) &&
						isNullOrEmpty(sf_correctiveaction)) {
					$label.addClass('OOC');
				} else {
					$label.removeClass('OOC');
				}

				var $prop;
				var propId = IdStr + '_input';

				if (!ReadOnly) {
					var addChangeHandler = true;

					switch (FieldType) {
					case "Date":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.date, ID: propId, value: sf_value });
						break;
					case "Link":
						$prop = $propDiv.CswLink('init', { ID: propId, href: sf_href, rel: 'external', value: sf_text });
						break;
					case "List":
						$prop = $('<select class="csw_prop_select" name="' + propId + '" id="' + propId + '"></select>')
							.appendTo($propDiv)
							.selectmenu();
						var selectedvalue = sf_value;
						var optionsstr = sf_options;
						var options = optionsstr.split(',');
						for (var i = 0; i < options.length; i++) {
							var $option = $('<option value="' + options[i] + '"></option>')
								.appendTo($prop);
							if (selectedvalue === options[i]) {
								$option.CswAttrDom('selected', 'selected');
							}

							if (!isNullOrEmpty(options[i])) {
								$option.val(options[i]);
							} else {
								$option.valueOf('[blank]');
							}
						}
						$prop.selectmenu('refresh');
						break;
					case "Logical":
						addChangeHandler = false; //_makeLogicalFieldSet() does this for us
						_makeLogicalFieldSet(ParentId, IdStr, sf_checked, sf_required)
							.appendTo($fieldcontain);
						break;
					case "Memo":
						$prop = $('<textarea name="' + propId + '">' + sf_text + '</textarea>')
							.appendTo($propDiv);
						break;
					case "Number":
						sf_value = tryParseNumber(sf_value, '');
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.number, ID: propId, value: sf_value });
						break;
					case "Password":
							//nada
						break;
					case "Quantity":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_value })
							.append(sf_units);
						break;
					case "Question":
						addChangeHandler = false; //_makeQuestionAnswerFieldSet() does this for us
						_makeQuestionAnswerFieldSet(ParentId, IdStr, sf_allowedanswers, sf_answer, sf_compliantanswers)
							.appendTo($fieldcontain);
						var hideComments = true;
						if (!isNullOrEmpty(sf_answer) &&
							(',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') < 0 &&
								isNullOrEmpty(sf_correctiveaction)) {
							$label.addClass('OOC');
							hideComments = false;
						}

						$prop = $('<div data-role="collapsible" class="csw_collapsible" data-collapsed="' + hideComments + '"><h3>Comments</h3></div>')
							.appendTo($retLi);

						var $corAction = $('<textarea id="' + IdStr + '_cor" name="' + IdStr + '_cor" placeholder="Corrective Action">' + sf_correctiveaction + '</textarea>')
							.appendTo($prop);

						if (sf_answer === '' || (',' + sf_compliantanswers + ',').indexOf(',' + sf_answer + ',') >= 0) {
							$corAction.css('display', 'none');
						}
						$corAction.unbind('change');
						$corAction.bind('change', function(eventObj) {
							var $cor = $(this);
							if ($cor.val() === '') {
								$label.addClass('OOC');
							} else {
								$label.removeClass('OOC');
							}
							onPropertyChange(ParentId, eventObj, $cor.val(), IdStr + '_cor', IdStr);
						});

						$('<textarea name="' + IdStr + '_input" id="' + IdStr + '_input" placeholder="Comments">' + sf_comments + '</textarea>')
							.appendTo($prop)
							.unbind('change')
							.bind('change', function(eventObj) {
								var $com = $(this);
								onPropertyChange(ParentId, eventObj, $com.val(), IdStr + '_com', IdStr);
							});
						break;
					case "Static":
						$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + sf_text + '</p>'));
						break;
					case "Text":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: propId, value: sf_text });
						break;
					case "Time":
						$prop = $propDiv.CswInput('init', { type: CswInput_Types.time, ID: propId, value: sf_value });
						break;
					default:
						$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
						break;
					} // switch (FieldType)

					if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
						$prop.unbind('change').bind('change', function(eventObj) {
							var $this = $(this);
							onPropertyChange(ParentId, eventObj, $this.val(), propId, Id);
						});
					}
				} else {
					$propDiv.append($('<p style="white-space:normal;" id="' + propId + '">' + json['gestalt'] + '</p>'));
				}
				if ($propDiv.children().length > 0) {
					$fieldcontain.append($propDiv);
				}
			}
			return $retLi;
		}

		function _FieldTypeHtmlToJson(json, elementId, propId, value) {
			/// <summary>
			///   Converts DOM content back to JSON
			/// </summary>
			/// <param name="json" type="Object">A JSON Object</param>
			/// <param name="elementId" type="String">The id of the DOM element</param>
			/// <param name="propId" type="String">The id of the property</param>
			/// <param name="value" type="String">The stored value</param>
			
			var elementName = new CswString(elementId);
			var propName = tryParseString(makeSafeId({ ID: json['id'] }), propId);
			var fieldtype = json['fieldtype'];
			//var propname = json.name;

			// subfield nodes
			var sf_text = 'text';
			var sf_value = 'value';
			//var $sf_href = json.href;
			//var $sf_options = json.options;
			var sf_checked = 'checked';
			//var $sf_required = json.required;
			//var $sf_units = json.units;
			var sf_answer = 'answer';
			//var $sf_allowedanswers = json.allowedanswers;
			var sf_correctiveaction = 'correctiveaction';
			var sf_comments = 'comments';
			//var $sf_compliantanswers = json.compliantanswers;

			var propToUpdate = '';
			switch (fieldtype) {
				case "Date":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Link":
					break;
				case "List":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Logical":
					if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
						propToUpdate = sf_checked;
					}
					break;
				case "Memo":
					if (elementName.contains(propName)) {
						propToUpdate = sf_text;
					}
					break;
				case "Number":
					if (elementName.contains(propName)) propToUpdate = sf_value;
					break;
				case "Password":
					break;
				case "Quantity":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				case "Question":
					if (elementName.contains(makeSafeId({ ID: propName, suffix: 'com' }))) {
						propToUpdate = sf_comments;
					} 
					else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
						propToUpdate = sf_answer;
					} 
					else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'cor' }))) {
						propToUpdate = sf_correctiveaction;
					}
					break;
				case "Static":
					break;
				case "Text":
					if (elementName.contains(propName)) {
						propToUpdate = sf_text;
					}
					break;
				case "Time":
					if (elementName.contains(propName)) {
						propToUpdate = sf_value;
					}
					break;
				default:
					break;
			}
			if (!isNullOrEmpty(propToUpdate)) {
				json[propToUpdate] = value;
				json['wasmodified'] = '1';
			}
		}// _FieldTypeHtmlToJson()

		function _makeLogicalFieldSet(ParentId, IdStr, Checked, Required) {
			var Suffix = 'ans';
			var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
										 .CswAttrDom({
										 'class': 'csw_fieldset',
										 'id': IdStr + '_fieldset'
									 })
										 .CswAttrXml({
										 'data-role': 'controlgroup',
										 'data-type': 'horizontal'
									 });
									 
			var answers = ['True', 'False'];
			if ( !isTrue(Required)) {
				answers.push = 'Null';
			}
			var inputName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unique and shared

			for (var i = 0; i < answers.length; i++) {
				var answertext = '';
				switch (answers[i]) {
				case 'Null':
					answertext = '?';
					break;
				case 'True':
					answertext = 'Yes';
					break;
				case 'False':
					answertext = 'No';
					break;
				}

				var inputId = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

				$fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
				var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] })

				// Checked is a Tristate, so isTrue() is not useful here
				if ((Checked === 'false' && answers[i] === 'False') ||
					(Checked === 'true' && answers[i] === 'True') ||
						(Checked === '' && answers[i] === 'Null')) {
					$input.CswAttrXml({ 'checked': 'checked' });
				}
				
				$input.unbind('change');
				$input.bind('change', function(eventObj) {
					var $this = $(this);
					var thisInput = $this.val();
					onPropertyChange(ParentId, eventObj, thisInput, inputId, IdStr);
					return false;
				});
			} // for (var i = 0; i < answers.length; i++)
			return $fieldset;
		}// _makeLogicalFieldSet()

		function _makeQuestionAnswerFieldSet(ParentId, IdStr, Options, Answer, CompliantAnswers) {
			var Suffix = 'ans';
			var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
										.CswAttrDom({
										'id': IdStr + '_fieldset'
									})
										.CswAttrXml({
										'data-role': 'controlgroup',
										'data-type': 'horizontal',
										'data-theme': 'b'
									});
			var answers = Options.split(',');
			var answerName = makeSafeId({ prefix: IdStr, ID: Suffix }); //Name needs to be non-unqiue and shared

			for (var i = 0; i < answers.length; i++) {
				var answerid = makeSafeId({ prefix: IdStr, ID: Suffix, suffix: answers[i] });

				$fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + answers[i] + '</label');
				var $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="csw_answer" value="' + answers[i] + '" />')
								.appendTo($fieldset);
				
				if (Answer === answers[i]) {
					 $answer.CswAttrDom('checked', 'checked');
				}
				$answer.unbind('change');
				$answer.bind('change', function(eventObj) {

					var thisAnswer = $(this).val();

					var correctiveActionId = makeSafeId({ prefix: IdStr, ID: 'cor' });
					var liSuffixId = makeSafeId({ prefix: IdStr, ID: 'label' });

					var $cor = $('#' + correctiveActionId);
					var $li = $('#' + liSuffixId);

					if ((',' + CompliantAnswers + ',').indexOf(',' + thisAnswer + ',') >= 0) {
						$cor.css('display', 'none');
						$li.removeClass('OOC');

					} else {
						$cor.css('display', '');

						if (isNullOrEmpty($cor.val())) {
							$li.addClass('OOC');
						} else {
							$li.removeClass('OOC');
						}
					}
					fixGeometry();
					onPropertyChange(ParentId, eventObj, thisAnswer, answerName, IdStr);

					return false;
				});
			} // for (var i = 0; i < answers.length; i++)
			return $fieldset;
		} // _makeQuestionAnswerFieldSet()
		
		function _addPageDivToBody(params) {
			var p = {
				ParentId: undefined,
				level: 1,
				DivId: '',       // required
				HeaderText: '',
				$toolbar: $(''),
				$content: $(''),
				HideSearchButton: false,
				HideOnlineButton: false,
				HideRefreshButton: false,
				HideHelpButton: false,
				HideBackButton: false,
				HideHeaderOnlineButton: true
			};

			if (params) {
				$.extend(p, params);
			}

			p.DivId = makeSafeId({ ID: p.DivId });

			var $pageDiv = $('#' + p.DivId);

			var firstInit = (isNullOrEmpty($pageDiv) || $pageDiv.length === 0);
			
			if (firstInit) {
				$pageDiv = $body.CswDiv('init', { ID: p.DivId })
					.CswAttrXml({
							'data-role': 'page',
							'data-url': p.DivId,
							'data-title': p.HeaderText,
							'data-rel': 'page'
						});
			}
			var headerDef = {
				buttons: {
					back: { ID: p.DivId + '_back',
								text: 'Back',
								cssClass: 'ui-btn-left',
								dataDir: 'reverse',
								dataIcon: 'arrow-l' },
					search: { ID: p.DivId + '_searchopen',
								text: 'Search',
								cssClass: 'ui-btn-right' }
				},
				ID: p.DivId,
			    text: p.HeaderText,
				dataId: 'csw_header',
				dataTheme: opts.Theme
			};
			var mobileHeader = new CswMobilePageHeader(headerDef, $pageDiv);

			mobileHeader.back.visible(!p.HideBackButton);
			mobileHeader.search.visible(!p.HideSearchButton);
				

			if(firstInit) {
				$pageDiv.CswDiv('init', { ID: p.DivId + '_content' })
					.CswAttrXml({ 'data-role': 'content', 'data-theme': opts.Theme })
					.append(p.$content);
			}

			var onlineValue = (!mobileStorage.amOnline()) ? 'Offline' : 'Online';
			var footerDef = {
				buttons: {
					online: { ID: p.DivId + '_gosyncstatus',
								text: onlineValue,
								cssClass: 'ui-btn-active onlineStatus ' + onlineValue.toLowerCase(),
								dataIcon: 'gear' },
					refresh: { ID: p.DivId + '_refresh',
								text: 'Refresh',
								cssClass: 'refresh',
								dataIcon: 'refresh' },
					fullsite: { ID: p.DivId + '_main',
								text: 'Full Site',
								href: 'Main.html', 
								rel: 'external',
								dataIcon: 'home' },
					help: { ID: p.DivId + '_help',
								text: 'Help',
								dataIcon: 'info' }
				},
				ID: p.DivId,
				dataId: 'csw_footer',
				dataTheme: opts.Theme
			};
			var mobileFooter = new CswMobilePageFooter(footerDef, $pageDiv);

			mobileFooter.online.visible(!p.HideOnlineButton);
			mobileFooter.refresh.visible(!p.HideRefreshButton && mobileStorage.amOnline());
			mobileFooter.help.visible(!p.HideHelpButton);
			
			if( mobileStorage.pendingChanges() ) {
				mobileFooter.online.addCssClass('pendingchanges');
			}
			else {
				mobileFooter.online.removeCssClass('pendingchanges');
			}
			
			//_bindPageEvents(p.DivId, p.ParentId, p.level, $pageDiv, mobileFooter, mobileHeader);
			return $pageDiv;

		}// _addPageDivToBody()
	
		// ------------------------------------------------------------------------------------
		// Sync Status Div
		// ------------------------------------------------------------------------------------

		function _makeSyncStatusDiv() {

		    var onlineDef = {
                theme: opts.Theme
		    };
		    var onlinePage = new CswMobilePageOnline(onlineDef, $body, mobileStorage, mobileSync);
		    var $ret = onlinePage.$pageDiv;
		    return $ret;
		}

		

		function _resetPendingChanges(succeeded) {
			if ( mobileStorage.pendingChanges() ) {
				$('.onlineStatus').addClass('pendingchanges')
								  .find('span.ui-btn-text')
								  .addClass('pendingchanges');
			} else {
				$('.onlineStatus').removeClass('pendingchanges')
								  .find('span.ui-btn-text')
								  .removeClass('pendingchanges');
			}
			
			if(arguments.length === 1) {
				if (succeeded) {
					mobileStorage.clearUnsyncedChanges();
					updatedUnsyncedChanges();
					$('#ss_lastsync_success').text(mobileStorage.lastSyncSuccess());
				}
				else {
					$('#ss_lastsync_attempt').text(mobileStorage.lastSyncAttempt());
				}
			}
		}

		// returns true if no pending changes or user is willing to lose them
		

		function _makeHelpDiv() {
			var helpDef = {
                theme: opts.Theme
		    };
		    var helpPage = new CswMobilePageHelp(helpDef, $body, mobileStorage);
		    var $ret = helpPage.$pageDiv;

			return $ret;
		}

	
		//#region Button Bindings
		
		function onRefresh() {
			var DivId = mobileStorage.currentViewId();
			if(isNullOrEmpty(DivId)) {
				window.location.reload();
			}
			else if (mobileStorage.amOnline() && 
				mobileStorage.checkNoPendingChanges() ) {
				
				if(DivId === 'viewsdiv') {
					window.location.reload();
				}
				else {
					var jsonData = {
						SessionId: SessionId,
						ParentId: DivId,
						ForMobile: ForMobile
					};

					CswAjaxJSON({
							formobile: ForMobile,
							url: opts.ViewUrl,
							data: jsonData,
							stringify: false,
							onloginfail: function(text) { onLoginFail(text, mobileStorage); },
							success: function(data) {
								setOnline(mobileStorage);
								if( !isNullOrEmpty(data['nodes']) ) {
									var viewJSON = data['nodes'];
									
									var params = {
										ParentId: 'viewsdiv',
										DivId: DivId,
										HeaderText: HeaderText,
										json: mobileStorage.updateStoredViewJson(DivId, viewJSON),
										parentlevel: 0,
										level: 1,
										HideRefreshButton: false,
										HideSearchButton: false,
										HideBackButton: false
									};
									params.onPageShow = function() { return _loadDivContents(params); };
									_loadDivContents(params).CswChangePage();
								}
							}, // success
							error: function() {
								onError();
							}
						});
				}
			}
		}


		//#endregion Button Bindings
		
		function onPropertyChange(DivId, eventObj, inputVal, inputId, inputPropId) {
			var logger = new CswProfileMethod('onPropertyChange');
			var $elm = $(eventObj.target);

			var name = tryParseString(inputId, $elm.CswAttrDom('id'));
			var value = tryParseString(inputVal, eventObj.target.innerText);
	   
			var nodeId = DivId.substr(DivId.indexOf('nodeid_nodes_'),DivId.length);
			var nodeJson = mobileStorage.fetchCachedNodeJson(nodeId);
			
			// update the xml and store it
			if (!isNullOrEmpty(nodeId) && !isNullOrEmpty(nodeJson)) {
				mobileStorage.addUnsyncedChange();
				_resetPendingChanges();
				
				if( !isNullOrEmpty(inputPropId) )
				{
					for (var key in nodeJson['subitems'])
					{
						var tab = nodeJson['subitems'][key];
						if (!isNullOrEmpty(tab[inputPropId])) {
							var prop = tab[inputPropId];
							_FieldTypeHtmlToJson(prop, name, inputPropId, value);
							//we're only updating one prop--don't iterate them all.
							break;
						}
					}
				}
				else { //remove else as soon as we can verify we never need to enter here
					errorHandler('Could not find a prop to update');
				}
				mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
			}
			mobileBgTask.reset();
			cacheLogInfo(logger);
		} // onPropertyChange()

		

	   
		
		function updatedUnsyncedChanges() {
			$('#ss_pendingchangecnt').text( tryParseString(mobileStorage.getItem('unSyncedChanges'),'0') );
		}

        //#region Page Construction		
		function bindMenuButtons(mobilePage) {
		    if (!isNullOrEmpty(mobilePage)) {
		        var header = mobilePage.mobileHeader;
		        if (!isNullOrEmpty(header)) {
		            for (var headBtnName in header.buttonNames) {
		                switch(headBtnName) {
		                    case 'back':
		                        break;
		                    case 'search':
		                        break;
		                }
		            }
		        }
		        var footer = mobilePage.mobileFooter;
		        if (!isNullOrEmpty(footer)) {
		            for (var footBtnName in footer.buttonNames) {
		                switch(footBtnName) {
		                    case 'online':
		                        break;
		                    case 'refresh':
		                        break;
		                    case 'help':
		                        break;
		                }
		            }
		        }
		    }
		}
	    //#endregion Page Construction
	    
		//#region Synchronization

		function processModifiedNodes(onSuccess)
		{
			if(!isNullOrEmpty(onSuccess)) {
				var modified = false;
				if (isNullOrEmpty(storedViews))
				{
					storedViews = mobileStorage.getItem('storedviews');
				}
				if (!isNullOrEmpty(storedViews))
				{
					for (var viewid in storedViews)
					{
						var view = mobileStorage.getItem(viewid);
						if (!isNullOrEmpty(view))
						{
							for (var nodeId in view['json'])
							{
								var node = mobileStorage.getItem(nodeId);
								if (!isNullOrEmpty(node) && node['wasmodified'])
								{
									modified = true;
									onSuccess(nodeId, node);
								}
							}
						}
					}
					if (!modified)
					{
						onSuccess();
					}
				}
			} else {
				_resetPendingChanges(true);
			}
		}
		
		function processUpdatedNodes(data,objectId,objectJSON,isBackgroundTask) {
			if( !isNullOrEmpty(data) ) {
			    setOnline(mobileStorage);
			    var completed = isTrue(data['completed']);
			    var isView = !isNullOrEmpty(data['nodes']);
			    if (isView)
			    {
			        var json = data['nodes'];
			        mobileStorage.updateStoredViewJson(objectId, json, false);
			    } else if (!completed)
			    {
			        mobileStorage.updateStoredNodeJson(objectId, objectJSON, false);
			    }

			    _resetPendingChanges(true);

			    if (completed && !isView)
			    {
			        mobileStorage.deleteNode(objectId, objectJSON['viewid']);
			        if (!isBackgroundTask)
			        {
			            $('#' + objectJSON['viewid']).CswChangePage();
			        }
			    }
			}
		}
		
		//#endregion Synchronization
		
		// For proper chaining support
		return this;
	};
})(jQuery);