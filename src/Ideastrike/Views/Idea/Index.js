(function () {
    "use strict";

    function updateVoteCount(data) {
        $("#votecount").text(data.NewVotes);
    }

    function vote(el) {
        var elem = $(el.currentTarget);
        var hasVoted = elem.data("voted").toLowerCase();

        if (hasVoted === "false") {
            $.ajax({
                type: "POST",
                url: "/idea/" + ideastrike.viewBag.ideaId + "/vote",
                context: document.body,
                success: function (data) {
                    updateVoteCount(data);
                    elem.data("voted", "true");
                    elem.text("unvote");
                }
            });
        } else {
            $.ajax({
                type: "POST",
                url: "/idea/" + ideastrike.viewBag.ideaId + "/unvote",
                context: document.body,
                success: function (data) {
                    updateVoteCount(data);
                    elem.data("voted", "false");
                    elem.text("vote");
                }
            });
        }
    }

    function loadFeed(elem, id) {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/idea/" + id + "/activity",
            context: document.body,
            success: function (data) {
                var count = data.Items.length;
                var converter = new Showdown.converter();

                for (var i = 0; i < count; i++) {
                    var node = data.Items[i];
                    var template = document.getElementById('comment_template').innerHTML;
                    if (node.template === "github")
                        template = document.getElementById('github_template').innerHTML;
                    else if (node.template === "admin")
                        template = document.getElementById('admin_template').innerHTML;

                    node.item.Text = converter.makeHtml(node.item.Text);

                    var output = Mustache.to_html(template, node.item);
                    $(elem).append(output);
                }
            }
        });
    }

    $(document).ready(function () {
        $("#vote").click(vote);
        $(".preview").click(ideastrike.previewText);
        $(".edit").click(ideastrike.editText);
        $(".hide-on-start").hide();
        $(".expander").click(ideastrike.toggleExpander).css("cursor", "pointer");
        loadFeed($("#activity-feed ol"), ideastrike.viewBag.ideaId);
        $('.fancybox').fancybox();
    });
} ());