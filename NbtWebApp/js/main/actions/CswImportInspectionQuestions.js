/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

var CswImportInspectionQuestions_WizardSteps = {
    'step1': { step: 1, description: 'Download template' },
    'step2': { step: 2, description: 'Select name and target' },
    'step3': { step: 3, description: 'Select File For Upload' },
    'step4': { step: 4, description: 'Preview' }
};

; (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswImportInspectionQuestions = function (options) 
	{
		var o = {
			ImportFileUrl: '/NbtWebApp/wsNBT.asmx/previewInspectionQuestions',
			viewid: '',
			viewname: '',
			viewmode: '',
			ID: 'importinspectionquestions',
			onCancel: function($wizard) {},
			onFinish: function($wizard) {},
			startingStep: 1
		};
		if(options) $.extend(o, options);

		var WizardStepArray = [ CswImportInspectionQuestions_WizardSteps.step1, CswImportInspectionQuestions_WizardSteps.step2, CswImportInspectionQuestions_WizardSteps.step3, CswImportInspectionQuestions_WizardSteps.step4];
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
				'Title': 'Import Inspection Questions',
				'StepCount': WizardStepArray.length,
				'Steps': WizardSteps,
				'StartingStep': o.startingStep,
				'FinishText': 'Save and Finish',
				'onNext': _handleNext,
				'onPrevious': _handlePrevious,
				'onCancel': o.onCancel,
				'onFinish': o.onFinish
			});

		// don't activate Save and Finish until step 2
		if(o.startingStep === 1)
			$wizard.CswWizard('button', 'finish', 'disable');

		// Step 1 - Download the Excel template
		var $div1 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step1.step);
        var downloadbutton = "<a href=\"/NbtWebApp/etc/InspectionDesign.xls\">Download the Excel template</a>";
		$div1.append(downloadbutton);

        // Step 2 - Get new inspection name and target type
		var $div2 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step2.step);
        var inspectionname = "<br/><br/>Please enter the name of the new inspection:<br />";
        $div2.append(inspectionname);
        var $TextBox = $div2.CswInput('init', {ID: o.ID + '_inspectionname', type: CswInput_Types.text});
        var $ErrorLabel = $('<div ID="InspectionNameErrorLabel" style="visibility:hidden">ERROR: inspection name is NOT unique.</div>');
        $div2.append($ErrorLabel);
        var targetname = "<br/><br/>Please enter the target of the new inspection:<br />";
        $div2.append(targetname);
        var $nodetypeselect = $('<div></div>');
        $nodetypeselect.CswNodeTypeSelect('init', {ID: 'step2_nodettypeselect'});
        //var $nodetypeselect = $div2.CswNodeTypeSelect('init', {ID: 'step2_nodettypeselect'});
        $div2.append($nodetypeselect);

        // step 3 - Upload file
		var $div3 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step3.step);
		var instructions3 = "<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>";
		$div3.append(instructions3);
		var $fileuploaddiv = $('<div></div>');

         // step 4 - Preview results
		var $div4 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step4.step);

       // define parameters to pass into FileUploader
		var o = {
			url: '/NbtWebApp/wsNBT.asmx/previewInspectionFile',
			params: 
            {
            InspectionName: _getNewInspectionName()
            },
			onSuccess: function(id, fileName, responseJSON) 
            { 
		        var instructions4b = "Inspection Name: ";
		        $div4.append(instructions4b);
                var $newinspectionname = _getNewInspectionName();
		        $div4.append($newinspectionname);
		        var instructions4c = "<br/><br/>Target: ";
		        $div4.append(instructions4c);
                 var $targetname = _getTargetInspection($nodetypeselect);
		        $div4.append($targetname);
		        var instructions4d = "<br/><br/>Your data from the Excel spreadsheet is shown below.  If this all looks correct then click the 'Save and Finish' button to create the new inspection.<br/><br/>";
		        $div4.append(instructions4d);

			    var step4previewgridid = o.ID + '_step4_previewgrid_outer';
                var $previewgrid = $div.find('#' + step4previewgridid);
                if (isNullOrEmpty($previewgrid) || $previewgrid.length === 0) {
                    $previewgrid = $('<div id="' + o.ID + '"></div>').appendTo($div4);
                } else {
                    $previewgrid.empty();
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

			    var grid = new CswGrid(g, $previewgrid);
            //_handleNext($wizard, CswImportInspectionQuestions_WizardSteps.step4.step);
            $wizard.CswWizard('button', 'next', 'click');
            }
		};

		var uploader = new qq.FileUploader({
			element: $fileuploaddiv.get(0),
			action: o.url,
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
		$div3.append($fileuploaddiv);
    
        function _getNewInspectionName()
        {
            // I am doing one basic step at a time to help with debugging
            var inspectionNameElement = $div2.find('#importinspectionquestions_inspectionname');
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

		function _handleNext($wizard, newstepno)
		{
			CurrentStep = newstepno;
			switch(newstepno)
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
			            'data': { 'NewInsepctionName': _getNewInspectionName() },
					    'success': function (response) { 
                            if (response.succeeded == 'true')
                            {
                                var control = document.getElementById('InspectionNameErrorLabel');
                                control.style.visibility = "hidden";
                                // Why does this not work?
                                //$('#InspectionNameErrorLabel').hide();
                            }
                            else 
                            {
                                var control = document.getElementById('InspectionNameErrorLabel');
                                control.style.visibility = "visible";
                                //$('#InspectionNameErrorLabel').show();
                			    $wizard.CswWizard('button', 'previous', 'click');
                            }
                        }
		            });
					break;
				case CswImportInspectionQuestions_WizardSteps.step4.step:
					$wizard.CswWizard('button', 'previous', 'enable');
					$wizard.CswWizard('button', 'next', 'disable');
 					$wizard.CswWizard('button', 'finish', 'enable');
					break;
			} // switch(newstepno)
		} // _handleNext()

		function _handlePrevious($wizard, newstepno)
		{
			CurrentStep = newstepno;
			switch(newstepno)
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
                    // dont think well ever have a step four in previous
 					$wizard.CswWizard('button', 'previous', 'enable');
 					$wizard.CswWizard('button', 'next', 'disable');
					$wizard.CswWizard('button', 'finish', 'enable');
					break;
			}
		}

		function _handleFinish($wizard)
		{
		} //_handleFinish

		return $div;
	} // $.fn.CswImportInspectionQuestions_WizardSteps
}) (jQuery);

