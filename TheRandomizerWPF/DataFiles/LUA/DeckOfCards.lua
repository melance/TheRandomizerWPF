local DeckOfCards = {}

CTHeaderStyle = [[font-family: Consolas, Courier, Monospace;
				  font-size: 9px;
				  background: white; 
			      cell-padding: 0; 
			      cell-spacing: 0; 
			      margin: 1em 0 0 1em; 
			      display: inline; 
			      border:1px solid black; 
			      border-collapse: collapse;]]
CTCellStyle	  = [[width:3em; 
				  height:0.75em;
				  text-align:center; 
				  vertical-align:center;]]
CTHeaderBlack = "<table style='" .. CTHeaderStyle .. " color:black;'>"
CTHeaderRed   = "<table style='" .. CTHeaderStyle .. " color:red;'>"
CTFooter	  = "</table>"
CTRowOpen	  = "<tr style='height:1em;'>"
CTRowClose	  = "</tr>"
CT_			  = "<td style='" .. CTCellStyle .. "'>&nbsp;</td>"
CTS			  = "<td style='" .. CTCellStyle .. "'>&spades;</td>"
CTH			  = "<td style='" .. CTCellStyle .. "'>&hearts;</td>"
CTD			  = "<td style='" .. CTCellStyle .. "'>&diams;</td>"
CTC			  = "<td style='" .. CTCellStyle .. "'>&clubs;</td>"
CTJ			  = "<td style='" .. CTCellStyle .. "'>J</td>"
CTQ			  = "<td style='" .. CTCellStyle .. "'>Q</td>"
CTK			  = "<td style='" .. CTCellStyle .. "'>K</td>"
CTA			  = "<td style='" .. CTCellStyle .. "'>A</td>"
CT1			  = "<td style='" .. CTCellStyle .. "'>10</td>"
CT2			  = "<td style='" .. CTCellStyle .. "'>2</td>"
CT3			  = "<td style='" .. CTCellStyle .. "'>3</td>"
CT4			  = "<td style='" .. CTCellStyle .. "'>4</td>"
CT5			  = "<td style='" .. CTCellStyle .. "'>5</td>"
CT6			  = "<td style='" .. CTCellStyle .. "'>6</td>"
CT7			  = "<td style='" .. CTCellStyle .. "'>7</td>"
CT8			  = "<td style='" .. CTCellStyle .. "'>8</td>"
CT9			  = "<td style='" .. CTCellStyle .. "'>9</td>"
CTO			  = "<td style='" .. CTCellStyle .. "'>O</td>"
CTE			  = "<td style='" .. CTCellStyle .. "'>E</td>"
CTR			  = "<td style='" .. CTCellStyle .. "'>R</td>"

SpadesA    = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTA..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CTA..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades2    = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT2..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT2..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades3	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT3..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT3..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades4	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT4..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT4..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades5	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT5..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT5..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades6	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT6..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT6..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades7	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT7..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT7..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades8	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT8..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT8..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades9	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT9..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT9..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Spades10   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT1..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CTS..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CT1..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
SpadesJ	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
SpadesQ	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTQ..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CTQ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
SpadesK	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTS..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTS..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
HeartsA    = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTA..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CTA..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts2    = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT2..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT2..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts3	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT3..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT3..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts4	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT4..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT4..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts5	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT5..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT5..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts6	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT6..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT6..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts7	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT7..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT7..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts8	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT8..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT8..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts9	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT9..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT9..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Hearts10   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT1..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CTH..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CT1..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
HeartsJ	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
HeartsQ	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTQ..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CTQ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
HeartsK	   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTH..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTH..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
ClubsA    = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTA..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CTA..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs2    = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT2..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT2..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs3	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT3..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT3..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs4	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT4..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT4..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs5	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT5..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT5..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs6	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT6..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT6..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs7	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT7..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT7..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs8	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT8..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT8..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs9	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT9..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT9..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Clubs10   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT1..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CTC..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CT1..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
ClubsJ	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
ClubsQ	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTQ..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CTQ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
ClubsK	   = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTC..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTC..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
DiamondsA    = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTA..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CTA..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds2    = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT2..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT2..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds3	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT3..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT3..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds4	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT4..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT4..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds5	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT5..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT5..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds6	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT6..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT6..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds7	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT7..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT7..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds8	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT8..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT8..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds9	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT9..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT9..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
Diamonds10   = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT1..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CTD..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CT1..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
DiamondsJ	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
DiamondsQ	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTQ..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CTQ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
DiamondsK	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTD..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CT_..CT_..CTD..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter

RedJoker	 = 	CTHeaderRed..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTO..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTE..CT_..CT_..CTRowClose..
				CTRowOpen..CTR..CT_..CTR..CTRowClose..
				CTRowOpen..CT_..CT_..CTE..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CTO..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter
BlackJoker	 = 	CTHeaderBlack..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTRowOpen..CTJ..CT_..CT_..CTRowClose..
				CTRowOpen..CTO..CT_..CT_..CTRowClose..
				CTRowOpen..CTK..CT_..CT_..CTRowClose..
				CTRowOpen..CTE..CT_..CT_..CTRowClose..
				CTRowOpen..CTR..CT_..CTR..CTRowClose..
				CTRowOpen..CT_..CT_..CTE..CTRowClose..
				CTRowOpen..CT_..CT_..CTK..CTRowClose..
				CTRowOpen..CT_..CT_..CTO..CTRowClose..
				CTRowOpen..CT_..CT_..CTJ..CTRowClose..
				CTRowOpen..CT_..CT_..CT_..CTRowClose..
				CTFooter				
				
function GetSpades()
	return {SpadesA,
			Spades2,
			Spades3,
			Spades4,
			Spades5,
			Spades6,
			Spades7,
			Spades8,
			Spades9,
			Spades10,
			SpadesJ,
			SpadesQ,
			SpadesK}
end

function GetHearts()
	return {HeartsA,
			Hearts2,
			Hearts3,
			Hearts4,
			Hearts5,
			Hearts6,
			Hearts7,
			Hearts8,
			Hearts9,
			Hearts10,
			HeartsJ,
			HeartsQ,
			HeartsK}
end
			
function GetClubs()
	return {ClubsA,
			Clubs2,
			Clubs3,
			Clubs4,
			Clubs5,
			Clubs6,
			Clubs7,
			Clubs8,
			Clubs9,
			Clubs10,
			ClubsJ,
			ClubsQ,
			ClubsK}
end

function GetDiamonds()
	return {DiamondsA,
		    Diamonds2,
            Diamonds3,
		    Diamonds4,
			Diamonds5,
			Diamonds6,
			Diamonds7,
			Diamonds8,
			Diamonds9,
			Diamonds10,
			DiamondsJ,
			DiamondsQ,
			DiamondsK}
end

function GetDeck()
	return {SpadesA,
			Spades2,
			Spades3,
			Spades4,
			Spades5,
			Spades6,
			Spades7,
			Spades8,
			Spades9,
			Spades10,
			SpadesJ,
			SpadesQ,
			SpadesK,
			HeartsA,
			Hearts2,
			Hearts3,
			Hearts4,
			Hearts5,
			Hearts6,
			Hearts7,
			Hearts8,
			Hearts9,
			Hearts10,
			HeartsJ,
			HeartsQ,
			HeartsK,
			ClubsA,
			Clubs2,
			Clubs3,
			Clubs4,
			Clubs5,
			Clubs6,
			Clubs7,
			Clubs8,
			Clubs9,
			Clubs10,
			ClubsJ,
			ClubsQ,
			ClubsK,
			DiamondsA,
			Diamonds2,
			Diamonds3,
			Diamonds4,
			Diamonds5,
			Diamonds6,
			Diamonds7,
			Diamonds8,
			Diamonds9,
			Diamonds10,
			DiamondsJ,
			DiamondsQ,
			DiamondsK}
end

function GetDeckWithJokers()
	return {SpadesA,
			Spades2,
			Spades3,
			Spades4,
			Spades5,
			Spades6,
			Spades7,
			Spades8,
			Spades9,
			Spades10,
			SpadesJ,
			SpadesQ,
			SpadesK,
			HeartsA,
			Hearts2,
			Hearts3,
			Hearts4,
			Hearts5,
			Hearts6,
			Hearts7,
			Hearts8,
			Hearts9,
			Hearts10,
			HeartsJ,
			HeartsQ,
			HeartsK,
			ClubsA,
			Clubs2,
			Clubs3,
			Clubs4,
			Clubs5,
			Clubs6,
			Clubs7,
			Clubs8,
			Clubs9,
			Clubs10,
			ClubsJ,
			ClubsQ,
			ClubsK,
			DiamondsA,
			Diamonds2,
			Diamonds3,
			Diamonds4,
			Diamonds5,
			Diamonds6,
			Diamonds7,
			Diamonds8,
			Diamonds9,
			Diamonds10,
			DiamondsJ,
			DiamondsQ,
			DiamondsK,
			RedJoker,
			BlackJoker}
end

return DeckOfCards