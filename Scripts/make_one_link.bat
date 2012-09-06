:: parameter 1: parent kiln directory
:: parameter 1: relative directory in which to make etc dir and links

cd %1%2
mkdir etc
cd etc
mklink CswDbConfig.xml %1\Common\CswConfigUI\bin\etc\CswDbConfig.xml
mklink CswSetupVbls.xml %1\Common\CswConfigUI\bin\etc\CswSetupVbls.xml
