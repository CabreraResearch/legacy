/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../clientdb/CswMobileClientDbResources.js" />
/// <reference path="../../CswProfileMethod.js" />
/// <reference path="../controls/CswMobileListView.js" />
/// <reference path="../fieldtypes/CswMobilePropFactory.js" />

//#region CswMobilePageProps

function CswMobilePageProps(propsDef, $page, mobileStorage) {
	/// <summary>
	///   Props Page class. Responsible for generating a Mobile props page.
	/// </summary>
    /// <param name="propsDef" type="Object">Props definitional data.</param>
	/// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
	/// <returns type="CswMobilePageProps">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private
    if (isNullOrEmpty(mobileStorage)) {
        mobileStorage = new CswMobileClientDbResources();
    }
    
    var pageDef = { };
    var id = CswMobilePage_Type.props.id;
    var title = CswMobilePage_Type.tabs.title;
    var viewId, level, nodeId, tabId, tabName, tabJson;
    var divSuffix = '_props';
    var ulSuffix = '_list';
    var $contentPage = $page.find('#' + id).find('div:jqmData(role="content")');
    var $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + id + divSuffix);
    var contentDivId;
    
    //ctor
    (function () {
        
        var p = {
	        level: 1,
	        ParentId: '',
            DivId: '', 
            viewId: mobileStorage.currentViewId(),
            tabId: mobileStorage.currentTabId(),
            tabName: '',
            nodeId: mobileStorage.currentNodeId(),
	        title: '',
	        theme: CswMobileGlobal_Config.theme,
            headerDef: { buttons: {} },
            footerDef: { buttons: {} },
	        onHelpClick: null, //function () {},
            onOnlineClick: null, //function () {},
            onRefreshClick: null, //function () {},
            onSearchClick: null //function () {}
        };
        if (propsDef) $.extend(p, propsDef);

        if (!isNullOrEmpty(p.DivId)) {
            id = p.DivId;
        } else {
            p.DivId = id;
        }

        contentDivId = id + divSuffix;
        
        if (!isNullOrEmpty(p.title)) {
            title = p.title;
        } else {
            p.title = title;
        }
        nodeId = p.nodeId;
        tabId = p.tabId;
        tabName = p.tabName;
        tabJson = p.tabJson;
        viewId = p.viewId;
        level = tryParseNumber(p.level, 2);
        
        var buttons = { };
        buttons[CswMobileFooterButtons.online.name] = p.onOnlineClick;
        buttons[CswMobileFooterButtons.refresh.name] = p.onRefreshClick;
        buttons[CswMobileFooterButtons.fullsite.name] = '';
        buttons[CswMobileFooterButtons.help.name] = p.onHelpClick;
        buttons[CswMobileHeaderButtons.back.name] = '';
        buttons[CswMobileHeaderButtons.search.name] = p.onSearchClick;

        pageDef = p = makeMenuButtonDef(p, id, buttons, mobileStorage);
        $content = ensureContent($content, contentDivId);
    })(); //ctor
   
    function getContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the tabs list from JSON</summary>
        ///<param name="onSuccess" type="Function">A function to execute after the list is built.</param>
        $content = ensureContent($content, contentDivId);
		if (!isNullOrEmpty(tabJson)) {
			refreshPropContent(onSuccess, postSuccess);
		} else {
			makeEmptyListView(null, $content, 'No Properties to Display');
		    stopLoadingMsg();
		}
    }
    
    function refreshPropContent(onSuccess, postSuccess) {
        ///<summary>Rebuilds the views list from JSON</summary>
        ///<param name="viewJson" type="Object">JSON representing a list of views</param>
        var ulDef = {
            ID: id + ulSuffix,
            cssclass: CswMobileCssClasses.listview.name,
            showLoading: false
        };
        
        var listView = new CswMobileListView(ulDef, $content, CswDomElementEvent.change);
        var propCount = 0;
        var nextTab;
        for (var propId in tabJson)
        {
            if(tabJson.hasOwnProperty(propId)) {
                var propJson = tabJson[propId];
                if (!isNullOrEmpty(propJson) && propId !== 'nexttab') {
                    var propName = propJson['prop_name'];
                    var ftDef = {
                        propId: propId,
                        propName: propName,
                        nodeId: nodeId,
                        tabId: tabId,
                        viewId: viewId
                    };
                    $.extend(ftDef, tabJson[propId]);
                    var prop = new CswMobilePropsFactory(ftDef);

                    var onChange = makeDelegate(onPropertyChange, {
                        propId: propId,
                        propName: propName,
                        controlId: prop.contentDivId,
                        onSuccess: ''
                    });
                    
                    var $li = listView.addListItemHtml(propId, prop.$label);
                    $li.append(prop.$content);
                    prop.applyFieldTypeLogicToContent($li);
                    $li.bind('change', onChange);

                } else {
                    nextTab = propJson;
                }
                propCount++;
            }
        }
        if (!isNullOrEmpty(nextTab)) {
            //remember to wire up click event
            listView.addListItemLink(makeSafeId({ ID: nextTab }), nextTab);    
        }
        if (propCount === 0) {
            makeEmptyListView(listView, $content, 'No Properties to Display');
        }
        if (!mobileStorage.stayOffline()) {
			toggleOnline(mobileStorage);
		}
        if (isFunction(onSuccess)) {
            onSuccess($content);
        }
        if (isFunction(postSuccess)) {
            postSuccess();
        }
    }
    
    function fieldTypeJsonToHtml(json,propId,propName,$li) {
		/// <summary>
		///   Converts JSON into DOM content
		/// </summary>
		/// <param name="json" type="Object">A JSON Object</param>  
		/// <param name="ParentId" type="String">The ElementID of the parent control (should be a prop)</param>
		/// <param name="IdStr" type="String">The ElementID of the child control</param>
        var $label = $('<h2 id="' + propId + '_label" style="white-space:normal;" class="csw_prop_label">' + propName + '</h2>')
                        .appendTo($li);
        var $fieldcontain = $('<div class="csw_fieldset" ></div>')
                        .appendTo($li);
		if( !isNullOrEmpty(json)) {
			var fieldType = json['fieldtype'];
			var readOnly = isTrue(json['isreadonly']);

			// Subfield values
			var sfText = tryParseString(json['text'], '');
			var sfValue = tryParseString(json['value'], '');
			var sfHref = tryParseString(json['href'], '');
			var sfChecked = tryParseString(json['checked'], '');
			//var sf_relationship = tryParseString(json['value']['name'], '');
			var sfRequired = tryParseString(json['required'], '');
			var sfUnits = tryParseString(json['units'], '');
			var sfAnswer = tryParseString(json['answer'], '');
			var sfAllowedanswers = tryParseString(json['allowedanswers'], '');
			var sfCorrectiveaction = tryParseString(json['correctiveaction'], '');
			var sfComments = tryParseString(json['comments'], '');
			var sfCompliantanswers = tryParseString(json['compliantanswers'], '');
			var sfOptions = tryParseString(json['options'], '');

		    var $propDiv = $('<div></div>');

			if (fieldType === "Question" &&
				!(sfAnswer === '' || (',' + sfCompliantanswers + ',').indexOf(',' + sfAnswer + ',') >= 0) &&
					isNullOrEmpty(sfCorrectiveaction)) {
				$label.addClass('OOC');
			} else {
				$label.removeClass('OOC');
			}

			var $prop;
			var elementId = propId + '_input';

			if (!readOnly) {
				var addChangeHandler = true;

				switch (fieldType) {
				case "Date":
					$prop = $propDiv.CswInput('init', { type: CswInput_Types.date, ID: elementId, value: sfValue });
					break;
				case "Link":
					$prop = $propDiv.CswLink('init', { ID: elementId, href: sfHref, rel: 'external', value: sfText });
					break;
				case "List":
					$prop = $('<select class="csw_prop_select" name="' + elementId + '" id="' + elementId + '"></select>')
						.appendTo($propDiv)
						.selectmenu();
					var selectedvalue = sfValue;
					var optionsstr = sfOptions;
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
					makeLogicalFieldSet(sfChecked, sfRequired, propId, propName)
						    .appendTo($fieldcontain);
					break;
				case "Memo":
					$prop = $('<textarea name="' + elementId + '">' + sfText + '</textarea>')
						.appendTo($propDiv);
					break;
				case "Number":
					sfValue = tryParseNumber(sfValue, '');
					$prop = $propDiv.CswInput('init', { type: CswInput_Types.number, ID: elementId, value: sfValue });
					break;
				case "Password":
						//nada
					break;
				case "Quantity":
					$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: elementId, value: sfValue })
						.append(sfUnits);
					break;
				case "Question":
					addChangeHandler = false; //_makeQuestionAnswerFieldSet() does this for us
					makeQuestionAnswerFieldSet(sfAllowedanswers, sfAnswer, sfCompliantanswers, propId, propName)
						        .appendTo($fieldcontain);
					var hideComments = true;
					if (!isNullOrEmpty(sfAnswer) &&
						(',' + sfCompliantanswers + ',').indexOf(',' + sfAnswer + ',') < 0 &&
							isNullOrEmpty(sfCorrectiveaction)) {
						$label.addClass('OOC');
						hideComments = false;
					}

					$prop = $('<div data-role="collapsible" class="csw_collapsible" data-collapsed="' + hideComments + '"><h3>Comments</h3></div>')
						.appendTo($li);

					var $corAction = $('<textarea id="' + propId + '_cor" name="' + propId + '_cor" placeholder="Corrective Action">' + sfCorrectiveaction + '</textarea>')
						.appendTo($prop);

					if (sfAnswer === '' || (',' + sfCompliantanswers + ',').indexOf(',' + sfAnswer + ',') >= 0) {
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
						onPropertyChange(tabId, eventObj, $cor.val(), propId + '_cor', id);
					});

					$('<textarea name="' + propId + '_input" id="' + propId + '_input" placeholder="Comments">' + sfComments + '</textarea>')
						.appendTo($prop)
						.unbind('change')
						.bind('change', function(eventObj) {
							var $com = $(this);
							onPropertyChange(tabId, eventObj, $com.val(), propId + '_com', propId);
						});
					break;
				case "Static":
					$propDiv.append($('<p style="white-space:normal;" id="' + elementId + '">' + sfText + '</p>'));
					break;
				case "Text":
					$prop = $propDiv.CswInput('init', { type: CswInput_Types.text, ID: elementId, value: sfText });
					break;
				case "Time":
					$prop = $propDiv.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: sfValue });
					break;
				default:
					$propDiv.append($('<p style="white-space:normal;" id="' + elementId + '">' + json['gestalt'] + '</p>'));
					break;
				} // switch (FieldType)

				if (addChangeHandler && !isNullOrEmpty($prop) && $prop.length !== 0) {
					var changeOpt = {
                        propId: propId,
                        propName: propName,
                        controlId: elementId,
                        onSuccess: makeDelegate(pageDef.onPropertyChange)
                    };
			    
				    $prop.unbind('change').bind('change', function(eventObj) {
						var $this = $(this);
						onPropertyChange(changeOpt);
					});
				}
			} else {
				$propDiv.append($('<p style="white-space:normal;" id="' + elementId + '">' + json['gestalt'] + '</p>'));
			}
			if ($propDiv.children().length > 0) {
				$fieldcontain.append($propDiv);
			}
		}
		return $fieldcontain;
	}
    
    function makeLogicalFieldSet(checked, required, propId, propName) {
		var Suffix = 'ans';
		var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
										.CswAttrDom({
										'class': 'csw_fieldset',
										'id': id + '_fieldset'
									})
										.CswAttrXml({
										'data-role': 'controlgroup',
										'data-type': 'horizontal'
									});
									 
		var answers = ['True', 'False'];
		if ( !isTrue(required)) {
			answers.push = 'Null';
		}
		var inputName = makeSafeId({ prefix: id, ID: Suffix }); //Name needs to be non-unique and shared

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

			var inputId = makeSafeId({ prefix: id, ID: Suffix, suffix: answers[i] });

			$fieldset.append('<label for="' + inputId + '">' + answertext + '</label>');
			var $input = $fieldset.CswInput('init', { type: CswInput_Types.radio, name: inputName, ID: inputId, value: answers[i] })

			// Checked is a Tristate, so isTrue() is not useful here
			if ((checked === 'false' && answers[i] === 'False') ||
				(checked === 'true' && answers[i] === 'True') ||
					(checked === '' && answers[i] === 'Null')) {
				$input.CswAttrXml({ 'checked': 'checked' });
			}
				
			$input.unbind('change');
			$input.bind('change', function(eventObj) {
				var $this = $(this);
				var thisInput = $this.val();
			    
			    var changeOpt = {
                    propId: propId,
                    propName: propName,
			        value: thisInput,
                    controlId: inputId,
                    onSuccess: makeDelegate(pageDef.onPropertyChange)
                };

				onPropertyChange(changeOpt);
				return false;
			});
		} // for (var i = 0; i < answers.length; i++)
		return $fieldset;
	}// _makeLogicalFieldSet()

	function makeQuestionAnswerFieldSet(options, answer, compliantAnswers, propId, propName) {
		var Suffix = 'ans';
		var $fieldset = $('<fieldset class="csw_fieldset"></fieldset>')
									.CswAttrDom({
									'id': id + '_fieldset'
								})
									.CswAttrXml({
									'data-role': 'controlgroup',
									'data-type': 'horizontal',
									'data-theme': 'b'
								});
		var answers = options.split(',');
		var answerName = makeSafeId({ prefix: id, ID: Suffix }); //Name needs to be non-unqiue and shared

		for (var i = 0; i < answers.length; i++) {
			var answerid = makeSafeId({ prefix: id, ID: Suffix, suffix: answers[i] });

			$fieldset.append('<label for="' + answerid + '" id="' + answerid + '_lab">' + answers[i] + '</label');
			var $answer = $('<input type="radio" name="' + answerName + '" id="' + answerid + '" class="csw_answer" value="' + answers[i] + '" />')
							.appendTo($fieldset);
				
			if (answer === answers[i]) {
					$answer.CswAttrDom('checked', 'checked');
			}
			$answer.unbind('change');
			$answer.bind('change', function(eventObj) {

				var thisAnswer = $(this).val();

				var correctiveActionId = makeSafeId({ prefix: id, ID: 'cor' });
				var liSuffixId = makeSafeId({ prefix: id, ID: 'label' });

				var $cor = $('#' + correctiveActionId);
				var $li = $('#' + liSuffixId);

				if ((',' + compliantAnswers + ',').indexOf(',' + thisAnswer + ',') >= 0) {
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
		    
			    var changeOpt = {
                    propId: propId,
                    propName: propName,
			        value: thisAnswer,
                    controlId: answerName,
                    onSuccess: makeDelegate(pageDef.onPropertyChange)
                };

				onPropertyChange(changeOpt);
				return false;
			});
		} // for (var i = 0; i < answers.length; i++)
		return $fieldset;
	} // _makeQuestionAnswerFieldSet()
    
    function fieldTypeHtmlToJson(propJson, elementId, propId, value) {
		/// <summary>
		///   Converts DOM content back to JSON
		/// </summary>
		/// <param name="json" type="Object">A JSON Object</param>
		/// <param name="elementId" type="String">The id of the DOM element</param>
		/// <param name="propId" type="String">The id of the property</param>
		/// <param name="value" type="String">The stored value</param>
			
		var elementName = new CswString(elementId);
		var propName = tryParseString(makeSafeId({ ID: propJson['id'] }), propId);
		var fieldtype = propJson['fieldtype'];
		//var propname = json.name;

		// subfield nodes
		var sfText = 'text';
		var sfValue = 'value';
		//var $sf_href = json.href;
		//var $sf_options = json.options;
		var sfChecked = 'checked';
		//var $sf_required = json.required;
		//var $sf_units = json.units;
		var sfAnswer = 'answer';
		//var $sf_allowedanswers = json.allowedanswers;
		var sfCorrectiveaction = 'correctiveaction';
		var sfComments = 'comments';
		//var $sf_compliantanswers = json.compliantanswers;

		var propToUpdate = '';
		switch (fieldtype) {
			case "Date":
				if (elementName.contains(propName)) {
					propToUpdate = sfValue;
				}
				break;
			case "Link":
				break;
			case "List":
				if (elementName.contains(propName)) {
					propToUpdate = sfValue;
				}
				break;
			case "Logical":
				if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
					propToUpdate = sfChecked;
				}
				break;
			case "Memo":
				if (elementName.contains(propName)) {
					propToUpdate = sfText;
				}
				break;
			case "Number":
				if (elementName.contains(propName)) propToUpdate = sfValue;
				break;
			case "Password":
				break;
			case "Quantity":
				if (elementName.contains(propName)) {
					propToUpdate = sfValue;
				}
				break;
			case "Question":
				if (elementName.contains(makeSafeId({ ID: propName, suffix: 'com' }))) {
					propToUpdate = sfComments;
				} 
				else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'ans' }))) {
					propToUpdate = sfAnswer;
				} 
				else if (elementName.contains(makeSafeId({ ID: propName, suffix: 'cor' }))) {
					propToUpdate = sfCorrectiveaction;
				}
				break;
			case "Static":
				break;
			case "Text":
				if (elementName.contains(propName)) {
					propToUpdate = sfText;
				}
				break;
			case "Time":
				if (elementName.contains(propName)) {
					propToUpdate = sfValue;
				}
				break;
			default:
				break;
		}
		if (!isNullOrEmpty(propToUpdate)) {
			propJson[propToUpdate] = value;
			propJson['wasmodified'] = '1';
		}
	}// _FieldTypeHtmlToJson()

	function onPropertyChange(options) {
	    var o = {
            propId: '',
	        propName: '',
	        controlId: '',
	        value: '',
	        onSuccess: ''
	    };
		if(options) {
		    $.extend(o, options);
		}
		var nodeJson = mobileStorage.fetchCachedNodeJson(nodeId);

		if (!isNullOrEmpty(nodeJson)) {
			mobileStorage.addUnsyncedChange();

		    var prop;
		    if (nodeJson.hasOwnProperty('subitems') &&
		        nodeJson.subitems.hasOwnProperty(tabName) &&
    		    nodeJson.subitems[tabName].hasOwnProperty(o.propId)) {
		        prop = nodeJson.subitems[tabName][o.propId];
		    }
		    
			if (!isNullOrEmpty(prop)) {
		    	fieldTypeHtmlToJson(prop, o.controlId, o.propId, o.value);
			    mobileStorage.updateStoredNodeJson(nodeId, nodeJson, '1');
			} else { //remove else as soon as we can verify we never need to enter here
				errorHandler('Could not find a prop to update');
			}
		}
	    if (isFunction(o.onSuccess)) {
	        o.onSuccess();
	    }
	} // onPropertyChange()
    
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

//#endregion CswMobilePageProps