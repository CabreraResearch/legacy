/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../_Global.js" />

var CswImportInspectionQuestions_WizardSteps = {
	'step1': { step: 1, description: 'Download template' },
	'step2': { step: 2, description: 'Select name and target' },
	'step3': { step: 3, description: 'Select File For Upload' },
	'step4': { step: 4, description: 'Preview' },
	'step5': { step: 5, description: 'Results' }
};

; (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswImportInspectionQuestions = function (options) 
	{
		var o = {
			ImportFileUrl: '',
			viewid: '',
			viewname: '',
			viewmode: '',
			ID: 'importInspectionQuestions',
			onCancel: function($wizard) {},
			onFinish: function($wizard) {},
			startingStep: 1
		};
		if(options) $.extend(o, options);

		var WizardStepArray = [ CswImportInspectionQuestions_WizardSteps.step1, CswImportInspectionQuestions_WizardSteps.step2, CswImportInspectionQuestions_WizardSteps.step3, CswImportInspectionQuestions_WizardSteps.step4, CswImportInspectionQuestions_WizardSteps.step5];
		var WizardSteps = {};                
		for( var i = 1; i <= WizardStepArray.length; i++ )
		{                
			WizardSteps[i] = WizardStepArray[i-1].description;
		}

		var CurrentStep = o.startingStep;

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		var $wizard = $div.CswWizard('init', { 
				'ID': o.ID + '_wizard',
				'Title': 'Create New Inspection',
				'StepCount': WizardStepArray.length,
				'Steps': WizardSteps,
				'StartingStep': o.startingStep,
				'FinishText': 'Finish',
				'onNext': _handleNext,
				'onPrevious': _handlePrevious,
				'onCancel': o.onCancel,
				'onFinish': o.onFinish
			});

		// don't activate Save and Finish until step 2
		if(o.startingStep === 1)
			$wizard.CswWizard('button', 'finish', 'disable');

		// Step 1 - Download the Excel template
		var $divStep1 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step1.step);
		var downloadButton = "<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Excel template</a>";
		$divStep1.append(downloadButton);

		// Step 2 - Get new inspection name and target type
		var $divStep2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);
		var inspectionName = "<br/><br/>Please enter the name of the new inspection:<br />";
		$divStep2.append(inspectionName);
		var $textBox = $divStep2.CswInput('init', {ID: o.ID + '_inspectionName', type: CswInput_Types.text});
		var $errorLabel = $('<div ID="inspectionNameErrorLabel" style="visibility:hidden">ERROR: inspection name is NOT unique.</div>');
		$divStep2.append($errorLabel);
		var targetName = "<br/><br/>Please enter the target of the new inspection:<br />";
		$divStep2.append(targetName);
		var $nodeTypeSelect = $('<div></div>');
		$nodeTypeSelect.CswNodeTypeSelect('init', {ID: 'step2_nodeTypeSelect'});
		//var $nodetypeselect = $div2.CswNodeTypeSelect('init', {ID: 'step2_nodettypeselect'});
		$divStep2.append($nodeTypeSelect);

		// step 3 - Upload file
		var $divStep3 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step3.step);
		var instructions3 = "<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>";
		$divStep3.append(instructions3);
		var $fileUploadDiv = $('<div></div>');

		 // step 4 - Preview results
		var $divStep4 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step4.step);

		 // step 4 - Results
		var $divStep5 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step5.step);

	   // define parameters to pass into FileUploader
		var o = {
			url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
			params: 
			{
    			InspectionName: _getNewInspectionName()
			},
			onSuccess: function(id, fileName, responseJSON) 
			{ 
		        var $textBoxTempFileName = $divStep4.CswInput('init', {ID: 'step4_tempFileName', type: CswInput_Types.hidden, value: responseJSON.tempFileName});
				var instructions4b = "Inspection Name: ";
				$divStep4.append(instructions4b);
				var $newInspectionName = _getNewInspectionName();
				$divStep4.append($newInspectionName);
				var instructions4c = "<br/><br/>Target: ";
				$divStep4.append(instructions4c);
				 var $targetName = _getTargetInspection($nodeTypeSelect);
				$divStep4.append($targetName);
				var instructions4d = "<br/><br/>Your data from the Excel spreadsheet is shown below.  If this all looks correct then click the 'Save and Finish' button to create the new inspection.<br/><br/>";
				$divStep4.append(instructions4d);

				var step4PreviewGridId = o.ID + '_step4_previewGrid_outer';
				var $previewGrid = $div.find('#' + step4PreviewGridId);
				if (isNullOrEmpty($previewGrid) || $previewGrid.length === 0) {
					$previewGrid = $('<div id="' + o.ID + '"></div>').appendTo($divStep4);
				} 
                else {
					$previewGrid.empty();
				}

				var g = {
					Id: o.ID,
					gridOpts: {
						autowidth: true,
						rowNum: 20
					},
					optNav: {
						add: false,
						view: false,
						del: false,
						refresh: false,
						edit: false
					}
				};

				$.extend(g.gridOpts, responseJSON.jqGridOpt);

				var grid = new CswGrid(g, $previewGrid);
			//_handleNext($wizard, CswImportInspectionQuestions_WizardSteps.step4.step);
			$wizard.CswWizard('button', 'next', 'click');
			}
		};

		var uploader = new qq.FileUploader({
			element: $fileUploadDiv.get(0),
			action: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
			params: o.params,
			debug: false,
			onSubmit: function()
			{
				o.params['InspectionName'] = _getNewInspectionName();
			},
			onComplete: function(id, fileName, responseJSON) 
			{ 
				o.onSuccess(id, fileName, responseJSON); 
			}
		});
		$divStep3.append($fileUploadDiv);
	
		function _getNewInspectionName()
		{
			// I am doing one basic step at a time to help with debugging
			var inspectionNameElement = $divStep2.find('#importInspectionQuestions_inspectionName');
			var inspectionNameValue = inspectionNameElement.val();
			return (inspectionNameValue);
		}

		function _getTargetInspection(targetInspectionSelect)
		{
			// I am doing one basic step at a time to help with debugging
			//var nodettypeselectElement = $div2.find('#step2_nodettypeselect');
			//var nodettypeselectValue = nodettypeselectElement.val();
			//return (nodettypeselectValue);
			return (targetInspectionSelect.find(':selected').text());
		}

		function _getTempFileName()
		{
			// I am doing one basic step at a time to help with debugging
			var tempFileNameElement = $divStep4.find('#step4_tempFileName');
			var tempFileNameValue = tempFileNameElement.val();
			return (tempFileNameValue);
		}

		function _handleNext($wizard, newStepNo)
		{
			CurrentStep = newStepNo;
			switch(newStepNo)
			{
				case CswImportInspectionQuestions_WizardSteps.step1.step:
					// dont think well ever have a step one in next
					$wizard.CswWizard('button', 'previous', 'disable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step2.step:
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step3.step:
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					CswAjaxJson({
						'url': '/NbtWebApp/wsNBT.asmx/IsNewInspectionNameUnique',
						'data': { 'NewInspectionName': _getNewInspectionName() },
						'success': function (response) { 
							if (response.succeeded == 'true')
							{
								var control = document.getElementById('inspectionNameErrorLabel');
								control.style.visibility = "hidden";
								// Why does this not work?
								//$('#InspectionNameErrorLabel').hide();
							}
							else 
							{
								var control = document.getElementById('inspectionNameErrorLabel');
								control.style.visibility = "visible";
								//$('#InspectionNameErrorLabel').show();
								$wizard.CswWizard('button', 'previous', 'click');
							}
						}
					});
					break;
				case CswImportInspectionQuestions_WizardSteps.step4.step:
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step5.step:
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'disable');
					$wizard.CswWizard('button', 'finish', 'enable');
                    var newInspectionName =  _getNewInspectionName();
                    var targetInspection =  _getTargetInspection($nodeTypeSelect);
                    var tempFileName =  _getTempFileName();

			        CswAjaxJson({
				        'url': '/NbtWebApp/wsNBT.asmx/uploadInspectionFile',
				        'data': { 
					        'NewInspectionName': newInspectionName, 
                            'TargetName': targetInspection, 
                            'TempFileName': tempFileName
				        },
				        'success': function (response) { 
					        if (response.success == 'true')
					        {
                                $wizard.CswWizard('button', 'previous', 'disable');
                                $wizard.CswWizard('button', 'cancel', 'disable');
                                $divStep5.append("Your design was created successfully");
					        }
					        else 
					        {
                                $divStep5.append("Error: " + response.error);
					        }
				        }
			        });
					break;
			} // switch(newstepno)
		} // _handleNext()

		function _handlePrevious($wizard, newStepNo)
		{
			CurrentStep = newStepNo;
			switch(newStepNo)
			{
				case CswImportInspectionQuestions_WizardSteps.step1.step: 
					$wizard.CswWizard('button', 'previous', 'disable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step2.step: 
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step3.step: 
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
				case CswImportInspectionQuestions_WizardSteps.step4.step: 
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'enable');
					$wizard.CswWizard('button', 'finish', 'disable');
					break;
			}
		}

		function _handleFinish($wizard)
		{
		} //_handleFinish

		return $div;
	} // $.fn.CswImportInspectionQuestions_WizardSteps
}) (jQuery);

