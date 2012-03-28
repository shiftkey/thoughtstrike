(function () {
    "use strict";
    $.contra(function () {
        $(function () {
            document.body.contentEditable = 'true';
            document.designMode = 'on';
            $('div').each(function () {
                var color = '#' + (0x1000000 + Math.random() * 0xFFFFFF).toString(16).substr(1, 6);
                $(this).css('backgroundColor', color);
            });
        });
    });
    $(document).ready(function () {
        $('#' + ideastrike.viewBag.selected).addClass('active');

        $(".vote").click(function () {
            var elem = $(this);
            var hasVoted = elem.data("voted").toLowerCase();

            if (hasVoted === "false") {
                $.ajax({
                    type: "POST",
                    url: "/idea/" + elem.data("id") + "/vote",
                    context: document.body,
                    success: function (data) {
                        $(".votecount." + elem.data("id")).text(data.NewVotes);
                        console.log(data.NewVotes);
                        elem.data("voted", "true");
                        elem.text("unvote");
                    }
                });
            } else {
                $.ajax({
                    type: "POST",
                    url: "/idea/" + elem.data("id") + "/unvote",
                    context: document.body,
                    success: function (data) {
                        $(".votecount." + elem.data("id")).text(data.NewVotes);
                        elem.data("voted", "false");
                        console.log(data.NewVotes);
                        elem.text("vote");
                    }
                });
            }
        });
    });
} ());