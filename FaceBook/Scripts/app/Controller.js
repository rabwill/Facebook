/* Rabia: our ng controller to call events */

app.controller('appController', ['$scope', 'wallService', function ($scope, wallService) {

    $scope.IsLoaded = false;
    /* Rabia:  call service to get all the posts */

    wallService.getPosts($scope);

    /* Rabia:  Make ng visible or hide it forever */

    $scope.IsLoaded = true;

    /* Rabia: Save post by calling the service to save it */

    $scope.savePostInWall = function () {
        var sub = {
            Message: $scope.newMessage
        };
        var saveposts = wallService.savePosts(sub);
        saveposts.then(function (d) {
            wallService.getPosts($scope);           
            $scope.newMessage = '';
        }, function (error) {
          
            console.log('Oops! We could not post such a nasty post ')
        })
    };

    /* Rabia: Save new comments by calling the service to save it */

    $scope.addComment = function (comment, id) {

       
        var comment = {
            Message: comment,
            PostId: id
        };
        var saveComments = wallService.addComment(comment);
        saveComments.then(function (d) {

            wallService.getPosts($scope);
            $scope.newCommentMessage = '';
        }, function (error) {
           
            console.log('Oops! We could not post such a nasty comment ')
        })
    };

    /* Rabia: toggle the post comment section */

    $scope.toggleComment = function (item, event) {
        $(event.target).next().find('.publishComment').toggle();
    }

}])


