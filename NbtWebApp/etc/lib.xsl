<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">


  <xsl:template match="*" priority="-5">
    <xsl:message>
      <xsl:text>Unhandled element: </xsl:text>
      <xsl:value-of select="local-name(..)"/>
      <xsl:text>/</xsl:text>
      <xsl:value-of select="local-name(.)"/>
    </xsl:message>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template name="makejavascriptsafe">
    <xsl:param name="string" select="''"/>

    <xsl:call-template name="replace">
      <xsl:with-param name="string" select="$string" />
      <xsl:with-param name="pattern"><xsl:text>'</xsl:text></xsl:with-param>
      <xsl:with-param name="replacement"><xsl:text>_quote_</xsl:text></xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <xsl:template name="makesafe">
    <xsl:param name="string" select="''"/>
    <xsl:param name="dofirstchar" select="'false'"/>
    
    <xsl:call-template name="makepunctuationsafe">
      <xsl:with-param name="string">
        <xsl:choose>
          <xsl:when test="$dofirstchar='true'">
            <xsl:call-template name="makefirstcharacternumbersafe">
              <xsl:with-param name="firstchar" select="substring( $string, 1, 1 )" />
            </xsl:call-template>
            <xsl:value-of select="substring( $string, 2, string-length( $string ))" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="$string"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  
  
  <xsl:template name="makepunctuationsafe">
    <xsl:param name="string" select="''"/>
      <xsl:call-template name="replace">
      <xsl:with-param name="string">
        <xsl:call-template name="replace">
          <xsl:with-param name="string">
            <xsl:call-template name="replace">
              <xsl:with-param name="string">
                <xsl:call-template name="replace">
                  <xsl:with-param name="string">
                    <xsl:call-template name="replace">
                      <xsl:with-param name="string">
                        <xsl:call-template name="replace">
                          <xsl:with-param name="string">
                            <xsl:call-template name="replace">
                              <xsl:with-param name="string">
                                <xsl:call-template name="replace">
                                  <xsl:with-param name="string">
                                    <xsl:call-template name="replace">
                                      <xsl:with-param name="string">
                                        <xsl:call-template name="replace">
                                          <xsl:with-param name="string" select="$string" />
                                          <xsl:with-param name="pattern"><xsl:text>?</xsl:text></xsl:with-param>
                                          <xsl:with-param name="replacement"><xsl:text>_qmark_</xsl:text></xsl:with-param>
                                        </xsl:call-template>
                                      </xsl:with-param>
                                      <xsl:with-param name="pattern"><xsl:text><![CDATA[&]]></xsl:text></xsl:with-param>
                                      <xsl:with-param name="replacement"><xsl:text>_amp_</xsl:text></xsl:with-param>
                                    </xsl:call-template>
                                  </xsl:with-param>
                                  <xsl:with-param name="pattern"><xsl:text>:</xsl:text></xsl:with-param>
                                  <xsl:with-param name="replacement"><xsl:text>_colon_</xsl:text></xsl:with-param>
                                </xsl:call-template>
                              </xsl:with-param>
                              <xsl:with-param name="pattern"><xsl:text>'</xsl:text></xsl:with-param>
                              <xsl:with-param name="replacement"><xsl:text>_quote_</xsl:text></xsl:with-param>
                            </xsl:call-template>
                          </xsl:with-param>
                          <xsl:with-param name="pattern"><xsl:text>,</xsl:text></xsl:with-param>
                          <xsl:with-param name="replacement"><xsl:text>_comma_</xsl:text></xsl:with-param>
                        </xsl:call-template>
                      </xsl:with-param>
                      <xsl:with-param name="pattern"><xsl:text>#</xsl:text></xsl:with-param>
                      <xsl:with-param name="replacement"><xsl:text>_num_</xsl:text></xsl:with-param>
                    </xsl:call-template>
                  </xsl:with-param>
                  <xsl:with-param name="pattern"><xsl:text>(</xsl:text></xsl:with-param>
                  <xsl:with-param name="replacement"><xsl:text>_lparen_</xsl:text></xsl:with-param>
                </xsl:call-template>
              </xsl:with-param>
              <xsl:with-param name="pattern"><xsl:text>)</xsl:text></xsl:with-param>
              <xsl:with-param name="replacement"><xsl:text>_rparen_</xsl:text></xsl:with-param>
            </xsl:call-template>
          </xsl:with-param>
          <xsl:with-param name="pattern"><xsl:text>/</xsl:text></xsl:with-param>
          <xsl:with-param name="replacement"><xsl:text>_slash_</xsl:text></xsl:with-param>
        </xsl:call-template>
      </xsl:with-param>
      <xsl:with-param name="pattern"><xsl:text xml:space="preserve"> </xsl:text></xsl:with-param>
      <xsl:with-param name="replacement"><xsl:text>_space_</xsl:text></xsl:with-param>
    </xsl:call-template>
  </xsl:template>
      
      
  <xsl:template name="makefirstcharacternumbersafe">
    <xsl:param name="firstchar" select="''"/>

    <xsl:call-template name="replace">
      <xsl:with-param name="string">
        <xsl:call-template name="replace">
          <xsl:with-param name="string">
            <xsl:call-template name="replace">
              <xsl:with-param name="string">
                <xsl:call-template name="replace">
                  <xsl:with-param name="string">
                    <xsl:call-template name="replace">
                      <xsl:with-param name="string">
                        <xsl:call-template name="replace">
                          <xsl:with-param name="string">
                            <xsl:call-template name="replace">
                              <xsl:with-param name="string">
                                <xsl:call-template name="replace">
                                  <xsl:with-param name="string">
                                    <xsl:call-template name="replace">
                                      <xsl:with-param name="string">
                                        <xsl:call-template name="replace">
                                          <xsl:with-param name="string" select="$firstchar" />
                                          <xsl:with-param name="pattern"><xsl:text>1</xsl:text></xsl:with-param>
                                          <xsl:with-param name="replacement"><xsl:text>_one_</xsl:text></xsl:with-param>
                                        </xsl:call-template>
                                      </xsl:with-param>
                                      <xsl:with-param name="pattern"><xsl:text>2</xsl:text></xsl:with-param>
                                      <xsl:with-param name="replacement"><xsl:text>_two_</xsl:text></xsl:with-param>
                                    </xsl:call-template>
                                  </xsl:with-param>
                                  <xsl:with-param name="pattern"><xsl:text>3</xsl:text></xsl:with-param>
                                  <xsl:with-param name="replacement"><xsl:text>_three_</xsl:text></xsl:with-param>
                                </xsl:call-template>
                              </xsl:with-param>
                              <xsl:with-param name="pattern"><xsl:text>4</xsl:text></xsl:with-param>
                              <xsl:with-param name="replacement"><xsl:text>_four_</xsl:text></xsl:with-param>
                            </xsl:call-template>
                          </xsl:with-param>
                          <xsl:with-param name="pattern"><xsl:text>5</xsl:text></xsl:with-param>
                          <xsl:with-param name="replacement"><xsl:text>_five_</xsl:text></xsl:with-param>
                        </xsl:call-template>
                      </xsl:with-param>
                      <xsl:with-param name="pattern"><xsl:text>6</xsl:text></xsl:with-param>
                      <xsl:with-param name="replacement"><xsl:text>_six_</xsl:text></xsl:with-param>
                    </xsl:call-template>
                  </xsl:with-param>
                  <xsl:with-param name="pattern"><xsl:text>7</xsl:text></xsl:with-param>
                  <xsl:with-param name="replacement"><xsl:text>_seven_</xsl:text></xsl:with-param>
                </xsl:call-template>
              </xsl:with-param>
              <xsl:with-param name="pattern"><xsl:text>8</xsl:text></xsl:with-param>
              <xsl:with-param name="replacement"><xsl:text>_eight_</xsl:text></xsl:with-param>
            </xsl:call-template>
          </xsl:with-param>
          <xsl:with-param name="pattern"><xsl:text>9</xsl:text></xsl:with-param>
          <xsl:with-param name="replacement"><xsl:text>_nine_</xsl:text></xsl:with-param>
        </xsl:call-template>
      </xsl:with-param>
      <xsl:with-param name="pattern"><xsl:text>0</xsl:text></xsl:with-param>
      <xsl:with-param name="replacement"><xsl:text>_zero_</xsl:text></xsl:with-param>
    </xsl:call-template>
  </xsl:template>


  <xsl:template name="replace">
    <xsl:param name="string" select="''"/>
    <xsl:param name="pattern" select="''"/>
    <xsl:param name="replacement" select="''"/>
    <xsl:choose>
      <xsl:when test="$pattern != '' and $string != '' and contains($string, $pattern)">
        <xsl:value-of select="substring-before($string, $pattern)"/>
        <!--
    Use "xsl:copy-of" instead of "xsl:value-of" so that users
    may substitute nodes as well as strings for $replacement.
    -->
        <xsl:copy-of select="$replacement"/>
        <xsl:call-template name="replace">
          <xsl:with-param name="string" select="substring-after($string, $pattern)"/>
          <xsl:with-param name="pattern" select="$pattern"/>
          <xsl:with-param name="replacement" select="$replacement"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>


