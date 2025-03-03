if (typeof $ !== 'undefined') {
    $(function () {
        // ! TODO: Required to load after DOM is ready, did this now with jQuery ready.
        window.Helpers.initSidebarToggle();
        // Toggle Universal Sidebar

        // Navbar Search with autosuggest (typeahead)
        // ? You can remove the following JS if you don't want to use search functionality.
        //----------------------------------------------------------------------------------

        var searchToggler = $('.search-toggler'),
            searchInputWrapper = $('.search-input-wrapper'),
            searchInput = $('.search-input1'),
            contentBackdrop = $('.content-backdrop');

        // Open search input on click of search icon
        if (searchToggler.length) {
            searchToggler.on('click', function () {
                if (searchInputWrapper.length) {
                    searchInputWrapper.toggleClass('d-none');
                    searchInput.focus();
                }
            });
        }
        // Open search on 'CTRL+/'
        $(document).on('keydown', function (event) {
            let ctrlKey = event.ctrlKey,
                slashKey = event.which === 191;

            if (ctrlKey && slashKey) {
                if (searchInputWrapper.length) {
                    searchInputWrapper.toggleClass('d-none');
                    searchInput.focus();
                }
            }
        });
        // Todo: Add container-xxl to twitter-typeahead
        searchInput.on('focus', function () {
            if (searchInputWrapper.hasClass('container-xxl')) {
                searchInputWrapper.find('.twitter-typeahead').addClass('container-xxl');
            }
        });

        if (searchInput.length) {
            // Filter config
            var filterConfig = function (data) {
                return function findMatches(q, cb) {
                    let matches;
                    matches = [];
                    data.filter(function (i) {
                       /* alert(i.employeeName);*/
                        if (i.employeeName.toLowerCase().startsWith(q.toLowerCase())) {
                            matches.push(i);
                        } else if (
                            !i.employeeName.toLowerCase().startsWith(q.toLowerCase()) &&
                            i.employeeName.toLowerCase().includes(q.toLowerCase())
                        ) {
                            matches.push(i);
                            matches.sort(function (a, b) {
                                return b.employeeName < a.employeeName ? 1 : -1;
                            });
                        } else {
                            return [];
                        }
                    });
                    cb(matches);
                };
            };

            // Search JSON
            var searchJson = 'search-vertical.json'; // For vertical layout
            if ($('#layout-menu').hasClass('menu-horizontal')) {
                var searchJson = 'search-horizontal.json'; // For vertical layout
            }
          
            // Search API AJAX call
            var searchData = $.ajax({
               url: "/EmployeeDirectory/GetEmployeeDirectory", //? Use your own search api instead
               dataType: 'json',
                async: false
            }).responseJSON;

           // alert(searchData.membersList.length);

            //var searchData = {
            //    members: [{ employeeName: 'Ahtesham', base64ProfileImage: '', jobTitle: 'ABC', id: 1 }, { employeeName: 'Ahtesham2', base64ProfileImage: '', jobTitle: 'xyz', id: 2 }, { employeeName: 'Ahtesham1', base64ProfileImage: '', jobTitle: '', id: 3 }]
            //}
            // Init typeahead on searchInput
            searchInput.each(function () {
                var $this = $(this);
                searchInput
                    .typeahead(
                        {
                            hint: false,
                            classNames: {
                                menu: 'tt-menu navbar-search-suggestion',
                                cursor: 'active',
                                suggestion: 'suggestion d-flex justify-content-between px-3 py-2 w-100'
                            }
                        },
                        
                        // Members
                        {
                            /*                            name: 'members',*/
                            name: 'members',
                            display: 'employeeName',
                            limit: 10,
                            /* source: filterConfig(searchData.members),*/
                            source: filterConfig(searchData.membersList),
                            templates: {
                                //header: '<h6 class="suggestions-header text-primary mb-0 mx-3 mt-3 pb-2">Members</h6>',
                                suggestion: function ({ employeeName, base64ProfileImage, jobTitle, id }) {
                                    //alert(employeeName);
                                    //alert(jobTitle);
                                   
                                    return '<a onclick="openPopup(' + id + ')" href="#"><div class="d-flex align-items-center"><img class="rounded-circle me-3" src="' + base64ProfileImage + '" alt="' + employeeName + '" height="35" width="35" ><div class="user-info"><h6 class="mb-0">' + employeeName + '</h6><small class="text-muted">' + jobTitle + "</small></div></div></a>";
                                    
                                },
                                notFound:
                                    '<div class="not-found px-3 py-2">' +
                                    //'<h6 class="suggestions-header text-primary mb-2">Members</h6>' +
                                    '<p class="py-2 mb-0"><i class="bx bx-error-circle bx-xs me-2"></i> No Results Found</p>' +
                                    '</div>'
                            }
                        }
                    )
                    //On typeahead result render.
                    .bind('typeahead:render', function () {
                        // Show content backdrop,
                        contentBackdrop.addClass('show').removeClass('fade');
                    })
                    // On typeahead select
                    .bind('typeahead:select', function (ev, suggestion) {
                        // Open selected page
                        if (suggestion.url) {
                            window.location = suggestion.url;
                        }
                    })
                    // On typeahead close
                    .bind('typeahead:close', function () {
                        // Clear search
                        searchInput.val('');
                        $this.typeahead('val', '');
                        // Hide search input wrapper
                        searchInputWrapper.addClass('d-none');
                        // Fade content backdrop
                        contentBackdrop.addClass('fade').removeClass('show');
                    });

                // On searchInput keyup, Fade content backdrop if search input is blank
                searchInput.on('keyup', function () {
                    if (searchInput.val() == '') {
                        contentBackdrop.addClass('fade').removeClass('show');
                    }
                });
            });

            // Init PerfectScrollbar in search result
            var psSearch;
            $('.navbar-search-suggestion').each(function () {
                psSearch = new PerfectScrollbar($(this)[0], {
                    wheelPropagation: false,
                    suppressScrollX: true
                });
            });

            searchInput.on('keyup', function () {
                psSearch.update();
            });
        }
    });
}


//function openPopup(id) {
//    alert(id);
//}