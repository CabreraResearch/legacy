using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30086 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 30086; }
        }

        public override void update()
        {
            // Insert license into licenses table
            CswTableUpdate LicenseUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "30086_license_update", "license" );
            DataTable LicenseTable = LicenseUpdate.getEmptyTable();
            DataRow lRow = LicenseTable.NewRow();
            lRow["activedate"] = DateTime.Now;
            lRow["licensetxt"] = @"These ""Terms of Use"" set forth the terms and conditions that apply to your use of ChemSWLive (the ""Web Site""). By using the Web Site (other than to read this page for the first time), you agree to comply with all of the Terms of Use set forth herein. The right to use the Web Site is personal to you and is not transferable to any other person or entity.
        
Copyrights and Trademarks
    A. All materials contained on the Web Site are Copyright 2006-2013, ChemSW, Inc. All rights reserved.
    B. No person is authorized to use, copy or distribute any portion the Web Site including related graphics.
    C. ChemSWLive and other trademarks and/or service marks (including logos and designs) found on the Web Site are trademarks/service marks that identify ChemSW, Inc. and the goods and/or services provided by ChemSW, Inc.. Such marks may not be used under any circumstances without the prior written authorization of ChemSW, Inc.
        
Links to Third-Party Web Site
ChemSW, Inc. may provide hyperlinks to third-party web sites as a convenience to users of the Web Site. ChemSW, Inc. does not control third-party web sites and is not responsible for the contents of any linked-to, third-party web sites or any hyperlink in a linked-to web site. ChemSW, Inc. does not endorse, recommend or approve any third-party web site hyperlinked from the Web Site. ChemSW, Inc. will have no liability to any entity for the content or use of the content available through such hyperlink.

No Representations or Warranties; Limitations on Liability
The information and materials on the Web Site could include technical inaccuracies or typographical errors. Changes are periodically made to the information contained herein. ChemSW, Inc. MAKES NO REPRESENTATIONS OR WARRANTIES WITH RESPECT TO ANY INFORMATION, MATERIALS OR GRAPHICS ON THE WEB SITE, ALL OF WHICH IS PROVIDED ON A STRICTLY ""AS IS"" BASIS, WITHOUT WARRANTY OF ANY KIND AND HEREBY EXPRESSLY DISCLAIMS ALL WARRANTIES WITH REGARD TO ANY INFORMATION, MATERIALS OR GRAPHICS ON THE WEB SITE, INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. UNDER NO CIRCUMSTANCES SHALL THE SITE OWNER OR PUBLISHER BE LIABLE UNDER ANY THEORY OF RECOVERY, AT LAW OR IN EQUITY, FOR ANY DAMAGES, INCLUDING WITHOUT LIMITATION, SPECIAL, DIRECT, INCIDENTAL, CONSEQUENTIAL OR PUNITIVE DAMAGES (INCLUDING, BUT NOT LIMITED TO LOSS OF USE OR LOST PROFITS), ARISING OUT OF OR IN ANY MANNER CONNECTED WITH THE USE OF INFORMATION OR SERVICES, OR THE FAILURE TO PROVIDE INFORMATION OR SERVICES, FROM THE WEB SITE.

Changes to These Terms of Use
ChemSW, Inc. reserves the right to change these Terms of Use at any time by posting new Terms of Use at this location. You can send e-mail to ChemSW, Inc. with any questions relating to these Terms of Use at enterprisesupport@chemsw.com.";

            LicenseTable.Rows.Add( lRow );
            LicenseUpdate.update( LicenseTable );
        }
    } // update()

}//namespace ChemSW.Nbt.Schema