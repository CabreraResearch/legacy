module('Csw.casno');
test('checkSumCASNo valid CASNo', function () {
    deepEqual(Csw.validateCASNo('12-34-0'), true, '12-34-0 is a valid CASNo');
});

test('checkSumCASNo invalid CASNo', function () {
    deepEqual(Csw.validateCASNo('12345'), false, '12340 is an invalid CASNo');
});

test('checkSumCASNo valid CASNo', function () {
    deepEqual(Csw.checkSumCASNo('12-34-0'), true, '12-34-0 has a valid checksum');
});

test('checkSumCASNo invalid CASNo', function () {
    deepEqual(Csw.checkSumCASNo('12-34-5'), false, '12-34-5 has an invalid checksum');
});