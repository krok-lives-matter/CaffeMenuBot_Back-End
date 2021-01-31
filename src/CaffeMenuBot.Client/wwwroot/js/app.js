window.blazorFunctions = {
  prepareNavBar: function () {
    $('#myTab a').on('click', function (e) {
      e.preventDefault()
      $(this).tab('show')
    })
    $('#categoryTab a').on('click', function (e) {
      e.preventDefault()
      $(this).tab('show')
    })
  },
  prepareCategoryTab: function () {
    if($(".delete-category-item")) {
      $(".delete-category-item").click(function() {
        var $row = $(this).closest("tr");
        var $text = $row.find(".nr").text();
        $("#modal-confirm .modal-body span").text($text)
      })
    }
  }
};