/*global Csw2:true*/
(function _Error(){


    var AssignmentError = Csw2.Class('AssignmentError', Error, function _AE(message, fileName, lineNumber) {
        var error = this;
        
        error.name = "AssignmentError";
        error.message = message || "Default Message";
        if(fileName ) {
            error.fileName = fileName;
        }
        if(lineNumber) {
            error.lineNumber= lineNumber;
        }
        return error;
    });

    Csw2.errors.lift('AssignmentError', function(message, fileName, lineNumber){
        return new AssignmentError(message, fileName, lineNumber);
    });

    var ClassInheritanceError = Csw2.Class('ClassInheritanceError', Error, function _CIE(message, fileName, lineNumber) {
        var error = this;
        
        error.name = "ClassInheritanceError";
        error.message = message || "Default Message";
        if(fileName ) {
            error.fileName = fileName;
        }
        if(lineNumber) {
            error.lineNumber= lineNumber;
        }
        return error;
    });

    Csw2.errors.lift('ClassInheritanceError', function(message, fileName, lineNumber){
        return new ClassInheritanceError(message, fileName, lineNumber);
    });

}());