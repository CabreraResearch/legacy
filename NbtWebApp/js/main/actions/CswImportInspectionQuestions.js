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
			ImportFileUrl: '/NbtWebApp/wsNBT.asmx/ImportInspectionQuestions',
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
        //$InspectionNameErrorLabel.style.visibility = "hidden";
        var targetname = "<br/><br/>Please enter the target of the new inspection:<br />";
        $div2.append(targetname);
        var $nodetypeselect = $('<div></div>');
        $nodetypeselect.CswNodeTypeSelect('init', {'ID': 'step2_nodettypeselect'});
        $div2.append($nodetypeselect);

        // step 3 - Upload file
		var $div3 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step3.step);
		var instructions3 = "<br/><br/>Please select your Excel file containing your inspection questions.<br/><br/>";
		$div3.append(instructions3);
		var $fileuploaddiv = $('<div></div>');

        // define parameters to pass into FileUploader
		var o = {
			url: '/NbtWebApp/wsNBT.asmx/uploadInspectionFile',
			params: 
            {
            InspectionName: _getNewInspectionName()
            },
			onSuccess: function(data) 
            { 
					var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
										.appendTo($div4);
					var $grid = $('<table id="'+ o.ID + '_gt" />')
										.appendTo($div4);

					var mygridopts = {
						'autowidth': true,
						'datatype': 'local', 
						'height': 180,
						'pager': $gridPager,
						'emptyrecords': 'No Results',
						'loadtext': 'Loading...',
						'multiselect': false,
						'rowList': [10,25,50],  
						'rowNum': 10
					} 
					
					var optNav = {
						'add': false,
						'view': false,
						'del': false,
						'refresh': false,
						'edit': false
					};
					$.extend(data.gridJson, mygridopts);

					$grid.jqGrid(data.gridJson)
						 .navGrid('#'+$gridPager.CswAttrDom('id'), optNav, {}, {}, {}, {}, {} ); 
					//$grid.jqGrid(gridJson)
					//	.hideCol('NODEID')
					//	.hideCol('NODEIDSTR');
            _handleNext($wizard, CswImportInspectionQuestions_WizardSteps.step4.step);
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
			onComplete: function() 
				{ 
					o.onSuccess(); 
				}
		});
		$div3.append($fileuploaddiv);
    
        // step 4 - Preview results
		var $div4 = $wizard.CswWizard('div', CswImportInspectionQuestions_WizardSteps.step4.step);
		var instructions4 = "<br/><br/>Here is a preview of your inspection file.<br/><br/>";
		$div4.append(instructions4);


        function _getNewInspectionName()
        {
            // I am doing one basic step at a time to help with debugging
            var inspectionNameElement = $div2.find('#importinspectionquestions_inspectionname');
            var inspectionNameValue = inspectionNameElement.val();
            return (inspectionNameValue);
        }

        function _getTargetInspection()
        {
            // I am doing one basic step at a time to help with debugging
            var inspectionNameElement = $div2.find('#step2_nodettypeselect');
            var inspectionNameValue = inspectionNameElement.val();
            return (inspectionNameValue);
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

