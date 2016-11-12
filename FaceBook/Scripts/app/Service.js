/* Rabia: ng service consumed by our controller */

var postApiUrl = 'api/WallPost/', commentApiUrl = 'api/Comment/';


app.service('wallService', ['$http', function ($http) {

    /* Rabia: function to get all post using webapi url */

    this.getPosts = function ($scope) {
        return $http({
            method: "GET",
            url: postApiUrl,
            headers: { 'Content-Type': 'application/json' }
        }).success(function (data) {
            /* Rabia: function to  get a post with the formatted date (year ago) and also to get the comments based on the post */
            var mappedPosts = $.map(data, function (item) { return new Post(item); });
            $scope.posts = mappedPosts;

            console.log(data);
        }).error(function (data) {
            console.log('Oops! That is unusual but we cant load any posts..');
        });;
    };

    /* Rabia: function to save a post using webapi url */

    this.savePosts = function (pos) {
        return $http(
        {
            method: 'post',
            data: pos,
            url: postApiUrl
        });
    }

    /* Rabia: function to add a comment within a post using webapi url */

    this.addComment = function (pos) {
        return $http(
        {
            method: 'post',
            data: pos,
            url: commentApiUrl
        });
    }

}])

 
/* Rabia: Process the posts   */

    function Post(data) {

    var $scope = this;
    data = data || {};

    $scope.PostId = data.PostId;

    $scope.Message = data.Message;
    $scope.PostedBy = data.PostedBy;
    $scope.PostedByName = data.PostedByName;
    $scope.PostedByAvatar = data.PostedByAvatar;
    $scope.PostedDate = getTimeAgo(data.PostedDate);
    $scope.error = "";
    $scope.PostComments = [];

    $scope.newCommentMessage = "";


    /*Rabia: If there are comments for the post load them after processing it (format date etc) */

    if (data.PostComments) {
        var mappedPostsComments = $.map(data.PostComments, function (item) { return new Comment(item); });
        $scope.PostComments = mappedPostsComments;


    }
    $scope.toggleComment = function (item, event) {
        $(event.target).next().find('.publishComment').toggle();
    }
}

/* Rabia:  Process the comments of posts  */

   function Comment(data) {
    var $scope = this;
    data = data || {};

    $scope.CommentId = data.CommentId;
    $scope.PostId = data.PostId;
    $scope.Message = data.Message;
    $scope.CommentedBy = data.CommentedBy;
    $scope.CommentedByAvatar = data.CommentedByAvatar;
    $scope.CommentedByName = data.CommentedByName;
    $scope.CommentedDate = getTimeAgo(data.CommentedDate);
    $scope.error = "";
}

/* Rabia:Time ago jquery function */

   function getTimeAgo(varDate) {
    if (varDate) {       
        var dt=varDate.toString().slice(-1) == 'Z' ? varDate : varDate + 'Z';
        var time = new Date(dt);
        return $.timeago(time);
    }
    else {
        return 'millions of years ago';
    }
}