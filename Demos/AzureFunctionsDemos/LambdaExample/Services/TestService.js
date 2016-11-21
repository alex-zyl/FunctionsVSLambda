function TestService() {
}

TestService.prototype.processEvent = function (event) {

    if (event != null) {
        console.log('event = ' + JSON.stringify(event));
    }
    else {
        console.log('No event object');

    }
};

module.exports = TestService;