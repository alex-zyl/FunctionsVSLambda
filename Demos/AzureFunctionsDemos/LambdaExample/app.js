var TestService = require("./Services/TestService");

exports.handler = function (event, context) {

    var service = new TestService();
    service.processEvent(event);
    
    context.done(null, 'Hello World');  // SUCCESS with message
};
